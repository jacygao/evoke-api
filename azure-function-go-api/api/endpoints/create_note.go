package endpoints

import (
	"context"
	"encoding/json"
	"net/http"
)

type Note struct {
	Content string `json:"content"`
}

var notes []Note

func CreateNote(ctx context.Context, req *http.Request) (*http.Response, error) {
	var note Note
	if err := json.NewDecoder(req.Body).Decode(&note); err != nil {
		return nil, err
	}
	notes = append(notes, note)
	return &http.Response{
		StatusCode: http.StatusCreated,
		Body:       http.NoBody,
	}, nil
}