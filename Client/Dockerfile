# Stage 1: Build Angular app
FROM node:20-alpine AS build
WORKDIR /app

# Копируем package.json и устанавливаем зависимости
COPY package*.json ./
RUN npm install

# Копируем исходники и собираем Angular
COPY . .
RUN npm run build --prod

# Stage 2: Nginx serve
FROM nginx:alpine

# Копируем сборку из предыдущего stage
COPY --from=build /app/dist/EventManagementSystemClient/browser /usr/share/nginx/html

# Копируем кастомную конфигурацию nginx (если есть)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]