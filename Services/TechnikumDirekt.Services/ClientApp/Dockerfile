### STAGE 1: Build ###
FROM node:alpine3.13 AS build
WORKDIR /usr/src/app
EXPOSE 80

COPY package.json package-lock.json ./

RUN npm cache clean --force
RUN npm i

COPY . .

RUN npm run build

FROM nginx as runtime
WORKDIR /usr/share/nginx/html
COPY --from=build /usr/src/app/dist .

### STAGE 2: Run ###
#FROM nginx:1.19.9-alpine
#WORKDIR /usr/share/nginx/html

#COPY --from=build /usr/src/app/dist .
#COPY --from=build /usr/src/app/nginx.conf /etc/nginx/nginx.conf

#CMD ["nginx", "-g", "daemon off;"]

#USER node

#RUN mkdir /home/node/.npm-global
#ENV PATH=/home/node/.npm-global/bin:$PATH
#ENV NPM_CONFIG_PREFIX=/home/node/.npm-global

#RUN npm install -g @angular/cli

#WORKDIR /src/app
#CMD ["ng", "serve", "--port", "4200", "--host", "0.0.0.0", "--disable-host-check", "--poll", "2000"]