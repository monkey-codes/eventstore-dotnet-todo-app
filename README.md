## Hotswap code changes
```
$ cd EventSourcing
$ dotnet watch run
```

## EventStoreDB

Run the db
```
$ docker-compose up
```

[Admin UI](http://localhost:2113/web/index.html#/dashboard)


## API

Create a new todo list

```
$ curl --request POST \
  --url https://localhost:5001/api/todolist \
  --header 'Content-Type: application/json' \
  --data '{
	"id": "cc3e7c41-9c82-4747-ab6d-8c284447e99a"
}'

// response
{
  "id": "19c3bcfa-58f8-4d4c-8c7c-495be0e2946c",
  "revision": 0,
  "response": null
}
```

To add items to the list the most recent revision number needs to be included in the 
command, this ensures that the list has not been
modified since the last read by this client. (Almost like optimistic locking)

```
$ curl --request POST \
  --url https://localhost:5001/api/TodoList/cc3e7c41-9c82-4747-ab6d-8c284447e99a/items \
  --header 'Content-Type: application/json' \
  --data '{
	"itemId": "9b709490-76e5-4949-b0ca-967a2064fc6e",
	"description": "task 1",
	"expectedRevision": 0
}'

// response

{
  "id": "9b709490-76e5-4949-b0ca-967a2064fc6e",
  "revision": 1,
  "response": null
}

```

## References
* https://github.com/evgeniy-khist/eventstoredb-event-sourcing
