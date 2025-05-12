package handlers

import (
    "net/http"
    "github.com/yourusername/azure-function-go-api/api/endpoints"
    "github.com/gorilla/mux"
)

func HandleGetNoteByID(w http.ResponseWriter, r *http.Request) {
    vars := mux.Vars(r)
    id := vars["id"]
    note, err := endpoints.GetNoteByID(id)
    if err != nil {
        http.Error(w, err.Error(), http.StatusNotFound)
        return
    }
    w.Header().Set("Content-Type", "application/json")
    w.WriteHeader(http.StatusOK)
    w.Write([]byte(note))
}