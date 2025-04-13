# ReadingIsGood

ReadingIsGood is an online books retail firm which operates only on the Internet. Main target of ReadingIsGood is to deliver books from its one centralized warehouse to their customers within the same day. That is why stock consistency is the first priority for their vision operations.

## Requirements
- .NET 8
- Docker

## Tech Stack
- PostgreSQL
- Redis-Stack
- SeriLog
- ElasticSearch

### For starting
1. Clone or download this repository  
2. Run `docker-compose up -d` in the project root folder  
3. After containers are up and running you can just go to this address:  
   **[http://localhost:8080/swagger](http://localhost:8080/swagger)**

### Included System Users For Test
- **Admin User Credentials**: `admin@admin.com` | `Ab12356**`  
  - Admin user can add/update books, see statistics  
- **Customer User Credentials**: `customer@customer.com` | `Ab12356**`  
  - Customer user can only see list of books and place new orders

> This project contains sample data for demonstration purposes.

## Features
- Centralized exception logging with **Serilog** and **ElasticSearch**  
- Error-proof custom exception implementation.  
- Added building blocks as a custom NuGet demonstration for a real world example.  
- Very simple and clean SOLID principle and DDD usage with onion architecture.

---

## Logging & ElasticSearch

We use **Serilog** for logging. All logs are shipped to **ElasticSearch** container defined in the `docker-compose.yml` file.  
- Default host for ElasticSearch is: `http://localhost:9200`  
- By default, logs are indexed to `readingisgood-logs-yyyy.MM`  
- If you have **Kibana** running, you can browse logs at `http://localhost:5601`.
