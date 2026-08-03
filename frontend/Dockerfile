# ---- Build stage ----
FROM node:22-alpine AS build
WORKDIR /app

# package*.json first, same caching reasoning as the backend Dockerfile -
# `npm ci` only re-runs when dependencies actually change.
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Vite inlines import.meta.env.* values into the compiled JS at build time,
# not at container runtime - so this has to be a build ARG, not a normal
# environment variable set on the running container (that would be too late;
# the bundle is already static files by then).
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN npm run build

# ---- Runtime stage ----
# The build output is static HTML/CSS/JS - no Node.js needed to serve it,
# just a plain web server.
FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
