# Entity Framework Relationships

### Create the Postgres database

Create a new Postgres docker container from the official image:

```sh
docker run -d --name books-webapi-postgres -p 5432:5432 -e POSTGRES_PASSWORD=My@Passw0rd postgres
```

Edit the DefaultConnection string in the `appsettings.json` file:

`"PostgresConnection": "Server=localhost; Database=books_db; Username=postgres; Password=My@Passw0rd;"`

In case you want to access the container, run this command:

```sh
docker exec -it books-webapi-postgres "bash"
```

Stop and remove the container when needed:

```sh
docker stop books-webapi-postgres && books-webapi-postgres rm sql1
```

### Migrations

Create the database based on the current migrations:

```sh
dotnet ef database update --project EntityFrameworkRelationships
```

If you need to add more migrations, run this command:

```sh
dotnet ef migrations add "MyNewMigration" --project EntityFrameworkRelationships
```

### Get Data

Use Postman or the browser to fetch some data:

[https://localhost:5001/api/books](https://localhost:5001/api/books)
