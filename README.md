# APEX — AI-Powered Fitness & Nutrition Platform

A portfolio-quality application demonstrating local/on-device AI (ONNX-based food detection and portion estimation), cloud-native architecture on AWS, and full-stack .NET development.

## Current status

**Phase 1 — Food tracking backend (in progress).** Backend-only: no frontend exists yet. The API is built and tested standalone via Swagger/OpenAPI UI, Postman, or curl.

See [`CLAUDE.md`](./CLAUDE.md) for the full project specification, architecture decisions, and development phases.

## Structure

```
apex-nutrition-platform/
├── apex-api/       ← ASP.NET Core 10 Web API (backend)
├── CLAUDE.md       ← project context and specification
└── README.md
```

## Tech stack (Phase 1)

- ASP.NET Core 10 / C# 14
- ONNX Runtime (YOLOv8 food detection, MiDaS depth estimation)
- OpenCvSharp4, FFMpegCore, SixLabors.ImageSharp
- AWS: S3, DynamoDB, Cognito, Lambda
- USDA FoodData Central API
