const DATA_URL = "frontend/data/tasks.json";
let tasks = [];

function showMessage(text, isError = false) {
    const box = document.getElementById("message-box");
    box.textContent = text;
    box.style.display = "block";
    box.style.backgroundColor = isError ? "#ff4d4d" : "#5DD62C";
}

async function loadTasks() {
    try {
        const localData = localStorage.getItem("my_tasks");
        if (localData) {
            tasks = JSON.parse(localData);
        } else {
            const response = await fetch(DATA_URL);
            if (!response.ok) {
                throw new Error(`Nie udało się wczytać danych (${response.status}).`);
            }
            tasks = await response.json();
            saveTasks();
        }
    } catch (error) {
        showMessage(error.message, true);
    }

    renderBoard();
}

function saveTasks() {
    localStorage.setItem("my_tasks", JSON.stringify(tasks));
}

function renderBoard() {
    const colNew = document.getElementById("col-new");
    const colInProgress = document.getElementById("col-in_progress");
    const colDone = document.getElementById("col-done");

    // Czyszczenie kolumn
    colNew.innerHTML = "";
    colInProgress.innerHTML = "";
    colDone.innerHTML = "";

    let counts = { new: 0, in_progress: 0, done: 0 };

    tasks.forEach(task => {
        counts[task.status] = (counts[task.status] || 0) + 1;

        const card = document.createElement("div");
        card.className = "card";

        const descText = task.description && task.description.trim() !== "" 
            ? task.description 
            : "brak opisu";

        card.innerHTML = `
            <div class="card-date">${task.date || 'Dziś'}</div>
            <div class="card-title">${task.title}</div>
            <div class="card-desc">● ${descText}</div>
            <div class="card-actions">
                <button onclick="editTask(${task.id})">Edytuj</button>
                <button class="delete-btn" onclick="deleteTask(${task.id})">Usuń</button>
            </div>
        `;

        if (task.status === "new") colNew.appendChild(card);
        else if (task.status === "in_progress") colInProgress.appendChild(card);
        else if (task.status === "done") colDone.appendChild(card);
    });

    document.getElementById("count-new").textContent = `(${counts.new || 0})`;
    document.getElementById("count-in_progress").textContent = `(${counts.in_progress || 0})`;
    document.getElementById("count-done").textContent = `(${counts.done || 0})`;
}

document.getElementById("task-form").addEventListener("submit", function(e) {
    e.preventDefault();

    const titleInput = document.getElementById("title");
    const descInput = document.getElementById("opis");
    const statusSelect = document.getElementById("status");

    const newTask = {
        id: Date.now(), // Unikalne ID
        title: titleInput.value,
        description: descInput.value,
        status: statusSelect.value,
        date: new Date().toLocaleDateString('pl-PL')
    };

    tasks.push(newTask);
    saveTasks();
    renderBoard();

    titleInput.value = "";
    descInput.value = "";
    statusSelect.value = "new";
});

function editTask(id) {
    const task = tasks.find(t => t.id === id);
    if (!task) return;

    const newTitle = prompt("Nowy tytuł:", task.title);
    if (newTitle === null) return; // Anulowano

    const newDesc = prompt("Nowy opis:", task.description);
    if (newDesc === null) return;

    const newStatus = prompt("Nowy status (new / in_progress / done):", task.status);
    if (newStatus === null) return;

    if (["new", "in_progress", "done"].includes(newStatus)) {
        task.title = newTitle;
        task.description = newDesc;
        task.status = newStatus;
        saveTasks();
        renderBoard();
    } else {
        alert("Nieprawidłowy status! Użyj: new, in_progress lub done.");
    }
}

function deleteTask(id) {
    if (confirm("Czy na pewno chcesz usunąć to zadanie?")) {
        tasks = tasks.filter(t => t.id !== id);
        saveTasks();
        renderBoard();
    }
}

window.addEventListener("DOMContentLoaded", loadTasks);