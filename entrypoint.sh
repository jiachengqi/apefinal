#!/usr/bin/env bash
set -e

PG_MAJOR=$(ls /usr/lib/postgresql | sort -n | tail -1)
PG_BIN="/usr/lib/postgresql/$PG_MAJOR/bin"
DATA_DIR="$PGDATA"

echo "Using Postgres $PG_MAJOR at $PG_BIN"
echo "Data directory: $DATA_DIR"

# Initialize database cluster on first run
if [ ! -s "$DATA_DIR/PG_VERSION" ]; then
  echo "Initializing Postgres data directory..."
  su postgres -c "$PG_BIN/initdb -D '$DATA_DIR'"

  echo "Starting Postgres for initial setup..."
  su postgres -c "$PG_BIN/postgres -D '$DATA_DIR' -c listen_addresses='127.0.0.1' -p 5432" &
  SETUP_PID=$!

  # Wait for Postgres startup
  until su postgres -c "$PG_BIN/pg_isready -h 127.0.0.1 -p 5432" > /dev/null 2>&1; do
    echo "Waiting for Postgres to start (setup)..."
    sleep 1
  done

  echo "Ensuring role ${POSTGRES_USER} exists..."
  su postgres -c "$PG_BIN/psql -v ON_ERROR_STOP=1 -h 127.0.0.1 -p 5432 -d postgres -tAc \"SELECT 1 FROM pg_roles WHERE rolname='${POSTGRES_USER}'\"" \
    | grep -q 1 || \
    su postgres -c "$PG_BIN/psql -v ON_ERROR_STOP=1 -h 127.0.0.1 -p 5432 -d postgres -c \"CREATE ROLE ${POSTGRES_USER} LOGIN PASSWORD '${POSTGRES_PASSWORD}';\""

  echo "Ensuring database ${POSTGRES_DB} exists..."
  su postgres -c "$PG_BIN/psql -v ON_ERROR_STOP=1 -h 127.0.0.1 -p 5432 -d postgres -tAc \"SELECT 1 FROM pg_database WHERE datname='${POSTGRES_DB}'\"" \
    | grep -q 1 || \
    su postgres -c "$PG_BIN/psql -v ON_ERROR_STOP=1 -h 127.0.0.1 -p 5432 -d postgres -c \"CREATE DATABASE ${POSTGRES_DB} OWNER ${POSTGRES_USER};\""

  echo "Stopping temporary Postgres..."
  kill "$SETUP_PID" || true
  wait "$SETUP_PID" || true
fi

echo "Starting Postgres for app..."
su postgres -c "$PG_BIN/postgres -D '$DATA_DIR' -c listen_addresses='127.0.0.1' -p 5432" &
PG_PID=$!

# Wait until Postgres is ready
until su postgres -c "$PG_BIN/pg_isready -h 127.0.0.1 -p 5432" > /dev/null 2>&1; do
  echo "Waiting for Postgres to become ready..."
  sleep 1
done

echo "Starting .NET app..."
dotnet apenew.dll &
APP_PID=$!

# When the app exits, stop Postgres
wait "$APP_PID"
echo "App exited, shutting down Postgres..."
kill "$PG_PID" || true
wait "$PG_PID" || true
