FROM python:3.12-slim AS final

WORKDIR /app

# Install system dependencies needed by pdfplumber and Pillow
RUN apt-get update && apt-get install -y --no-install-recommends \
  libjpeg-dev \
  zlib1g-dev \
  libpng-dev \
  curl \
  && rm -rf /var/lib/apt/lists/*

# Create non-root user
# RUN groupadd -r flaskuser && useradd -r -g flaskuser flaskuser
RUN groupadd -r flaskuser && useradd -r -g flaskuser -d /app flaskuser

# Install Python dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade pip \
  && pip install --no-cache-dir -r requirements.txt

# Copy application code
COPY flask_app.py .

# Change ownership
RUN chown -R flaskuser:flaskuser /app
USER flaskuser

EXPOSE 5050

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:5050/health || exit 1

#here with the worker 4 sync timeout of close is 30 and each request waits 6 second before closing the connection , after 500 request it flush randomly  log the error, and port is 5000 and entry point is app
CMD ["gunicorn", \
  "--workers", "4", \
  "--worker-class", "sync", \
  "--timeout", "30", \
  "--graceful-timeout", "25", \
  "--keep-alive", "6", \
  "--max-requests", "500", \
  "--max-requests-jitter", "50", \
  "--access-logfile", "-", \
  "--error-logfile", "-", \
  "--bind", "0.0.0.0:5050", \
  "flask_app:app"]
