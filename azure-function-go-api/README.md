# Azure Function Go API

This project is an Azure Function App built using Go. It provides a simple API for managing notes with the following endpoints:

## Endpoints

1. **Create Note**
   - **Method:** POST
   - **Path:** `/notes`
   - **Request Body:** 
     - `content` (string): The content of the note to be created.
   - **Description:** Creates a new note with the provided content.

2. **Get All Notes**
   - **Method:** GET
   - **Path:** `/notes`
   - **Description:** Retrieves a list of all notes.

3. **Get Note by ID**
   - **Method:** GET
   - **Path:** `/notes/{id}`
   - **Description:** Retrieves a specific note by its ID.

## Setup Instructions

1. **Install Go**
   - Ensure you have Go installed on your machine. You can download it from [golang.org](https://golang.org/dl/).

2. **Clone the Repository**
   - Clone this repository to your local machine using:
     ```
     git clone <repository-url>
     ```

3. **Navigate to the Project Directory**
   - Change into the project directory:
     ```
     cd azure-function-go-api
     ```

4. **Install Dependencies**
   - Run the following command to install the necessary dependencies:
     ```
     go mod tidy
     ```

5. **Deploy to Azure**
   - Follow the Azure documentation to deploy your Go function app.

## Running the Application Locally

To run the application locally, use the following command:
```
go run main.go
```

## License

This project is licensed under the MIT License. See the LICENSE file for details.