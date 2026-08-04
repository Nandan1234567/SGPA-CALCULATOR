#!/bin/bash
set -e

DOMAIN="${1:?Usage: ./scripts/init-ssl.sh vtucalc.in shettynandan071@gmail.com}"
EMAIL="${2:?Usage: ./scripts/init-ssl.sh vtucalc.in shettynandan071@gmail.com}"
COMPOSE="docker compose -f docker-compose.prod.yml --env-file .env"

echo "=== Step 1: Starting Nginx ==="
$COMPOSE up -d nginx
sleep 5

echo "=== Step 2: Verifying Nginx is up ==="
curl -sf http://localhost/health-nginx || {
  echo "ERROR: Nginx not responding"
  $COMPOSE logs nginx
  exit 1
}

echo "=== Step 3: Getting SSL certificate ==="
$COMPOSE run --rm certbot certonly \
  --webroot \
  --webroot-path=/var/www/certbot \
  --email "$EMAIL" \
  --agree-tos \
  --no-eff-email \
  -d "$DOMAIN" \
  -d "www.$DOMAIN"

echo "=== Step 4: Downloading TLS parameters ==="
mkdir -p ./certbot/conf

if [ ! -f "./certbot/conf/options-ssl-nginx.conf" ]; then
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot-nginx/certbot_nginx/_internal/tls_configs/options-ssl-nginx.conf \
    > ./certbot/conf/options-ssl-nginx.conf
fi

if [ ! -f "./certbot/conf/ssl-dhparams.pem" ]; then
  curl -s https://raw.githubusercontent.com/certbot/certbot/master/certbot/certbot/ssl-dhparams.pem \
    > ./certbot/conf/ssl-dhparams.pem
fi

echo "=== Step 5: Switching to SSL config ==="
cp nginx/conf.d/app.ssl.conf nginx/conf.d/app.conf

echo "=== Step 6: Reloading Nginx ==="
$COMPOSE exec nginx nginx -s reload

echo "Done. SSL is live."
