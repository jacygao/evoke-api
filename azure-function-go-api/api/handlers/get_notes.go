package handlers

import (
    "net/http"
    "github.com/yourusername/azure-function-go-api/api/endpoints"
)

func HandleGetNotes(w http.ResponseWriter, r *http.Request) {
    notes, err := endpoints.GetNotes()
    if err != nil {
        http.Error(w, err.Error(), http.StatusInternalServerError)
        return
    }

    w.Header().Set("Content-Type", "application/json")
    w.WriteHeader(http.StatusOK)
    w.Write(notes)
}