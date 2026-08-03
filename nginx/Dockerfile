# ── Stage 1: Build React App ─────────────────────────────────────────────────
FROM node:24-alpine AS frontend-builder
WORKDIR /app

# context is root (.), so prefix all frontend paths
COPY frontend/package.json frontend/package-lock.json* ./
RUN npm ci

COPY frontend/ .

ARG VITE_API_BASE_URL=""
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

# ── Stage 2: Nginx serves React + proxies API ─────────────────────────────────
FROM nginx:1.27-alpine AS final

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx/nginx.conf /etc/nginx/nginx.conf


COPY --from=frontend-builder /app/dist /usr/share/nginx/html

RUN mkdir -p /var/www/certbot

EXPOSE 80 443
# In nginx/Dockerfile, change HEALTHCHECK to:
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost/health-nginx 2>/dev/null || wget -qO- http://localhost/health-nginx || exit 1

CMD ["nginx", "-g", "daemon off;"]


