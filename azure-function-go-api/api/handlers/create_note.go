package handlers

import (
	"context"
	"net/http"
	"azure-function-go-api/api/endpoints"
	"encoding/json"
)

func HandleCreateNote(w http.ResponseWriter, r *http.Request) {
	var content string
	if err := json.NewDecoder(r.Body).Decode(&content); err != nil {
		http.Error(w, "Invalid request payload", http.StatusBadRequest)
		return
	}

	note, err := endpoints.CreateNote(content)
	if err != nil {
		http.Error(w, "Failed to create note", http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusCreated)
	json.NewEncoder(w).Encode(note)
}