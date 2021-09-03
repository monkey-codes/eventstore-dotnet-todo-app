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

List all todo lists

```
$ curl --request GET \
  --url https://localhost:5001/api/todolist \
  --header 'Content-Type: application/json'
  
 // response 

[
  {
    "id": "88bce914-bfc0-499a-9f12-7beba879f25c",
    "numberOfTasks": 0,
    "revision": 0
  },
  {
    "id": "6ff10606-78df-4db2-b50e-11e4eb94cebd",
    "numberOfTasks": 3,
    "revision": 3
  },
  {
    "id": "f1f3ffa8-dac9-46fe-9f77-1eae45f9ece5",
    "numberOfTasks": 0,
    "revision": 0
  }
]

```

Get a specific TodoList

```
$ curl --request GET \
  --url https://localhost:5001/api/todolist/6ff10606-78df-4db2-b50e-11e4eb94cebd \
  --header 'Content-Type: application/json'

// response

{
  "id": "6ff10606-78df-4db2-b50e-11e4eb94cebd",
  "tasks": [
    {
      "taskId": "fae0f902-fc43-4f17-aec1-8ddc03b8bc48",
      "description": "task 1"
    },
    {
      "taskId": "66f6f33d-ef82-4cec-9558-eec8fc40e11f",
      "description": "task 2"
    },
    {
      "taskId": "66e7e1ee-f84d-44b4-af18-99c89af38353",
      "description": "task 3"
    }
  ],
  "revision": 3
}

```
## References
* https://github.com/evgeniy-khist/eventstoredb-event-sourcing
