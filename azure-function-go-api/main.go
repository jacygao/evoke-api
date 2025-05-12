package main

import (
	"azure-function-go-api/api/handlers"

	"github.com/Azure/azure-functions-go/azfunc"
)

func main() {
	azfunc.Start(azfunc.Functions{
		"CreateNote":  azfunc.HTTPHandler(handlers.HandleCreateNote),
		"GetNotes":    azfunc.HTTPHandler(handlers.HandleGetNotes),
		"GetNoteByID": azfunc.HTTPHandler(handlers.HandleGetNoteByID),
	})
}
