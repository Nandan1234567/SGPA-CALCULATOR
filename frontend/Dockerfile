# ── Stage 1: Build React App ─────────────────────────────────────────────────
FROM node:24-alpine AS frontend-builder
WORKDIR /app

COPY frontend/package.json frontend/package-lock.json* ./
RUN npm ci

COPY frontend/ .

ARG VITE_API_BASE_URL=""
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

# ── Stage 2: Nginx serves React + proxies API ─────────────────────────────────
FROM nginx:1.27-alpine AS final

RUN rm /etc/nginx/conf.d/default.conf

# Custom nginx.conf with rate limiting zones — MUST come before app.conf
# app.ssl.conf references limit_req zone=api which is defined here
COPY nginx/nginx.conf /etc/nginx/nginx.conf

COPY nginx/conf.d/app.conf /etc/nginx/conf.d/app.conf

COPY --from=frontend-builder /app/dist /usr/share/nginx/html

# Certbot writes ACME challenge files here during SSL cert acquisition
RUN mkdir -p /var/www/certbot

EXPOSE 80 443

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget -qO- http://localhost/health-nginx || exit 1

CMD ["nginx", "-g", "daemon off;"]
