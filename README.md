# [Basic Dev Tools - https://basictools.dev](https://basictools.dev/)
[![Continuous Integration](https://github.com/NArnott/BasicTools/actions/workflows/ci.yml/badge.svg)](https://github.com/NArnott/BasicTools/actions/workflows/ci.yml)

A website that provides basic, online developer tools

Ever had that need to create handle of GUIDs for a project? Need to format or validate parse a JSON document? This project is meant to solve those basic needs.

The site is built using AspNetCore Blazor. The majority of the site is built using Blazor WebAssembly. However, to solve the SEO (Search Engine Optimization) issues inherent with SPA websites, the WebAssembly site is hosted from a AspNetCore Server, which provides pre-rendering support, as well as other service functions, such as a sitemap.

The entire site is packaged as a Docker Container and hosted using the AWS cloud:
* **ECR** for Docker Image storage
* **ECS** for running the Docker Image in Fargate
* **ALB** for load-balancing against the ECS tasks
* **CloudFront** for caching and quick access to users
* **Route53** for DNS management
