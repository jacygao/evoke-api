package endpoints

import (
    "net/http"
    "encoding/json"
)

var notes = []string{"Note 1", "Note 2", "Note 3"}

func GetNotes(w http.ResponseWriter, r *http.Request) {
    w.Header().Set("Content-Type", "application/json")
    w.WriteHeader(http.StatusOK)
    json.NewEncoder(w).Encode(notes)
}