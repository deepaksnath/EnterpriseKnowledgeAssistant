# DPK.EKA (Enterprise Knowledge Assistant)

## Overview

DPK.EKA (Enterprise Knowledge Assistant) is a .NET 10-based sample application that demonstrates a Retrieval-Augmented Generation (RAG) style knowledge assistant. The solution is split into projects for API surface, application logic, domain models and infrastructure integration. The service uses Azure OpenAI for chat/generation and Azure Cognitive Search for vector/semantic search over ingested documents.

Functional capabilities
- Accept natural language questions via a simple REST endpoint.
- Perform vector search against an Azure Cognitive Search index to retrieve relevant documents.
- Use Azure OpenAI chat/completion and embeddings to produce answers augmented by retrieved context.

Technical summary
- Solution targets .NET 10.
- Projects:
  - `DPK.EKA.Api` - ASP.NET Core Web API and dependency registration.
  - `DPK.EKA.Application` - Application services, models and interfaces.
  - `DPK.EKA.Infrastructure` - Infrastructure integrations (storage, search, OpenAI clients).
  - `DPK.EKA.Domain` - Domain models and entities.
- Uses the Azure SDKs: `Azure.AI.OpenAI` and `Azure.Search.Documents`.
- API exposes a controller `RagController` with POST `api/rag/query` to ask questions.

## Required Azure resources
Create the following Azure resources before running the application:

1. Resource Group (any name)
2. Azure OpenAI resource (or equivalent Azure OpenAI endpoint)
   - Create a deployment for chat (set a `ChatDeployment` name).
   - Create a deployment for embeddings (set an `EmbeddingDeployment` name).
   - Obtain endpoint URL and key.
3. Azure Cognitive Search
   - Create a search service, obtain the Search endpoint and admin API key.
   - Create a search index to store documents and embeddings. The sample configuration expects an index name (default in code: `dpk-eka-index`).

Note: You can create resources using the Azure Portal, Azure CLI, or ARM templates. The app expects the endpoint URLs and keys to be provided in configuration (see next section).

## Configure application settings
Update `DPK.EKA.Api/appsettings.json` (or use user-secrets / environment variables) with your Azure resource values under the `AzureAiSettings` section. Example keys to set:

- `AzureOpenAiEndpoint` - Azure OpenAI endpoint URL (e.g. `https://<your-openai>.openai.azure.com/`)
- `AzureOpenAiApiKey` - Key for Azure OpenAI
- `ChatDeployment` - name of your chat deployment
- `EmbeddingDeployment` - name of your embedding deployment
- `SearchEndpoint` - Azure Cognitive Search endpoint (e.g. `https://<service>.search.windows.net`)
- `SearchApiKey` - admin key for Cognitive Search
- `SearchIndexName` - name of search index (e.g. `dpk-eka-index`)

Important: The repository contains sample placeholder values in `appsettings.json`. Replace them with your own keys and endpoints before running.

## Create the search index (basic guidance)
You can create the index using the Azure Portal UI for your Cognitive Search service or using the REST API. The index must include fields for document id, content and vector embeddings. A minimal index might include:

- `id` (key, Edm.String)
- `content` (Edm.String, searchable)
- `metadata` (Edm.String)
- `embedding` (Collection(Edm.Single))

If you prefer the REST approach, use the Search REST API with your admin key. Consult Azure Cognitive Search docs for example index schemas.

## Clone, build and run locally
1. Clone the repository:

   `git clone https://github.com/deepaksnath/EnterpriseKnowledgeAssistant.git`

2. Open a terminal and change to the solution folder:

   `cd EnterpriseKnowledgeAssistant`

3. (Optional) Edit `DPK.EKA.Api/appsettings.json` or set secrets/environment variables with your Azure values.

4. Build the solution:

   `dotnet build`

5. Run the API (from the solution root or `DPK.EKA.Api`):

   `cd DPK.EKA.Api`
   `dotnet run`

6. When the app starts, Kestrel will display the listening URLs (for example `https://localhost:5001`).

## Test the API (Swagger and Postman)

Swagger UI
- While running in the Development environment, Swagger is enabled. Open the URL printed by the app and browse to `/swagger` (for example `https://localhost:5001/swagger`).
- Use the `POST /api/rag/query` endpoint to send questions.

Example request body (JSON):

`{ "question": "What is the purpose of this assistant?" }`

Postman / cURL
- POST to the endpoint `https://localhost:5001/api/rag/query` with header `Content-Type: application/json` and the JSON body above.

curl example:

`curl -k -X POST https://localhost:5001/api/rag/query -H "Content-Type: application/json" -d "{ \"question\": \"What is the purpose of this assistant?\" }"`

Notes
- The API will return a JSON response with the generated answer. Actual response format is produced by the RAG service and may include additional metadata.
- If you receive errors, verify that the Azure endpoints, keys and index name in `appsettings.json` are correct.

## API Endpoints
This application exposes the following HTTP endpoints. All endpoints are under the base path `/api`.

- POST `/api/rag/query`
  - Description: Submit a natural language question. The service will create an embedding for the question, perform a vector search to retrieve relevant document chunks, and call the chat LLM to produce a context-aware answer.
  - Content-Type: `application/json`
  - Request body example:

    `{ "question": "What is the purpose of this assistant?" }`

  - Successful response: `200 OK` with JSON body (example):

    `{ "answer": "...generated text...", "sources": ["doc1.pdf","doc2.pdf"] }`

  - Error responses: `400 Bad Request` for missing question, `500 Internal Server Error` for processing issues.

- POST `/api/documents/upload`
  - Description: Upload a PDF document to be ingested. The service will extract text chunks, create embeddings and upload them to the configured Azure Cognitive Search index.
  - Content-Type: `multipart/form-data`
  - Form field: `file` (the PDF file)
  - Notes: Only PDF is accepted by the current implementation. There is a file size check (configured in code).
  - Successful response: `200 OK` with JSON body of ingestion result (example):

    `{ "fileName": "example.pdf", "chunksIndexed": 12 }`

  - Error responses: `400 Bad Request` if file missing/invalid, `500 Internal Server Error` for ingestion failures.

Usage tips
- Use Swagger UI to test both endpoints interactively.
- For `documents/upload` in Postman, select `form-data` body type and add a `file` field with type `File`.
- Monitor logs for detailed errors; the controllers log and return appropriate status codes.

## Docker (optional)
The project contains a `DPK.EKA.Api/Dockerfile`. Build and run as follows from repo root:

`docker build -t dpk-eka -f DPK.EKA.Api/Dockerfile .`
`docker run -p 5000:80 -e ASPNETCORE_ENVIRONMENT=Production -e AzureOpenAi__AzureOpenAiApiKey="<key>" dpk-eka`

Update environment variables passed to the container to provide your Azure configuration.

## Troubleshooting
- If the app fails to bind to Azure OpenAI or Cognitive Search, check keys and endpoint URLs.
- When using HTTPS and self-signed dev certificates, you may need to accept the dev certificate in your browser or use `-k`/`--insecure` with curl.

## Contributing and further work
- This repo is a sample to demonstrate RAG patterns with Azure services. You can extend ingestion, indexing, metadata handling and conversation memory to suit your needs.

---
Generated and maintained by the project skeleton. For specific implementation details, inspect the source under `DPK.EKA.Api`, `DPK.EKA.Application` and `DPK.EKA.Infrastructure`.
