const API_URL = "http://localhost:5113/api/tasks";
let tasks = [];

// error/success
function showMessage(text, isError = false) {
    const box = document.getElementById("message-box");
    if (!box) return;
    box.textContent = text;
    box.style.display = "block";
    box.style.backgroundColor = isError ? "#ff4d4d" : "#5DD62C";
    box.style.color = isError ? "#ffffff" : "#000000";
}

async function request(url, options = {}) {
    let response;
    try {
        response = await fetch(url, {
            headers: { "Content-Type": "application/json" },
            ...options,
        });
    } catch {
        throw new Error("Brak połączenia z serwerem. Sprawdź, czy API działa.");
    }

    if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || `Błąd serwera (${response.status})`);
    }

    if (response.status === 204) {
        return null;
    }
    return response.json();
}

// GET
async function loadTasks() {
    try {
        tasks = await request(API_URL);
        renderBoard();
    } catch (error) {
        showMessage(error.message, true);
    }
}

// textContent & Render
function renderBoard() {
    const colNew = document.getElementById("col-new");
    const colInProgress = document.getElementById("col-in_progress");
    const colDone = document.getElementById("col-done");

    if (!colNew || !colInProgress || !colDone) return;

    colNew.innerHTML = "";
    colInProgress.innerHTML = "";
    colDone.innerHTML = "";

    let counts = { new: 0, in_progress: 0, done: 0 };

    tasks.forEach(task => {
        counts[task.status] = (counts[task.status] || 0) + 1;

        const card = document.createElement("div");
        card.className = "card";

        const titleDiv = document.createElement("div");
        titleDiv.className = "card-title";
        titleDiv.textContent = task.title; // Защита от XSS

        const descText = task.description && task.description.trim() !== "" 
            ? task.description 
            : "brak opisu";
            
        const descDiv = document.createElement("div");
        descDiv.className = "card-desc";
        descDiv.textContent = "● " + descText; // Защита от XSS

        const actionsDiv = document.createElement("div");
        actionsDiv.className = "card-actions";

        // Кнопка редактирования (Edytuj)
        const editBtn = document.createElement("button");
        editBtn.textContent = "Edytuj";
        editBtn.style.backgroundColor = "#1f6feb";
        editBtn.style.color = "white";
        editBtn.onclick = () => editTask(task.id);
        actionsDiv.appendChild(editBtn);

        // done/in_progress-button
        if (task.status !== "done") {
            const doneBtn = document.createElement("button");
            doneBtn.textContent = "Zakończ";
            doneBtn.onclick = () => updateTaskStatus(task.id, "done");
            actionsDiv.appendChild(doneBtn);
        }

        // del-button
        const deleteBtn = document.createElement("button");
        deleteBtn.className = "delete-btn";
        deleteBtn.textContent = "Usuń";
        deleteBtn.onclick = () => deleteTask(task.id);
        actionsDiv.appendChild(deleteBtn);

        card.appendChild(titleDiv);
        card.appendChild(descDiv);
        card.appendChild(actionsDiv);

        if (task.status === "new") colNew.appendChild(card);
        else if (task.status === "in_progress") colInProgress.appendChild(card);
        else if (task.status === "done") colDone.appendChild(card);
    });

    document.getElementById("count-new").textContent = `(${counts.new || 0})`;
    document.getElementById("count-in_progress").textContent = `(${counts.in_progress || 0})`;
    document.getElementById("count-done").textContent = `(${counts.done || 0})`;
}

// POST (Добавление задачи с userId: 2 для C# бэкенда)
document.getElementById("task-form").addEventListener("submit", async function(e) {
    e.preventDefault();

    const titleInput = document.getElementById("title");
    const descInput = document.getElementById("opis");
    const statusSelect = document.getElementById("status");

    const data = {
        title: titleInput.value.trim(),
        description: descInput.value.trim() || null,
        status: statusSelect.value,
        userId: 2 
    };

    if (!data.title) {
        showMessage("Tytuł jest wymagany.", true);
        return;
    }

    try {
        const created = await request(API_URL, {
            method: "POST",
            body: JSON.stringify(data)
        });
        showMessage(`Dodano zadanie "${created.title}".`);
        this.reset();
        await loadTasks();
    } catch (error) {
        showMessage(error.message, true);
    }
});

// EDIT (Редактирование задачи)
async function editTask(id) {
    const task = tasks.find(t => t.id === id);
    if (!task) return;

    const newTitle = prompt("Nowy tytuł:", task.title);
    if (newTitle === null) return; // Если нажали "Отмена"

    if (!newTitle.trim()) {
        showMessage("Tytuł jest wymagany.", true);
        return;
    }

    const newDesc = prompt("Nowy opis:", task.description || "");
    if (newDesc === null) return;

    try {
        await request(`${API_URL}/${id}`, {
            method: "PUT",
            body: JSON.stringify({
                title: newTitle.trim(),
                description: newDesc.trim() || null,
                status: task.status,
                userId: task.userId || 2
            })
        });
        showMessage("Zaktualizowano zadanie.");
        await loadTasks();
    } catch (error) {
        showMessage(error.message, true);
    }
}

// PUT (Обновление только статуса)
async function updateTaskStatus(id, newStatus) {
    try {
        const task = tasks.find(t => t.id === id);
        await request(`${API_URL}/${id}`, {
            method: "PUT",
            body: JSON.stringify({
                title: task.title,
                description: task.description,
                status: newStatus,
                userId: task.userId || 2
            })
        });
        showMessage("Zaktualizowano status zadania.");
        await loadTasks();
    } catch (error) {
        showMessage(error.message, true);
    }
}

// DELETE
async function deleteTask(id) {
    if (!confirm("Na pewno usunąć zadanie?")) return;
    try {
        await request(`${API_URL}/${id}`, {
            method: "DELETE"
        });
        showMessage("Usunięto zadanie.");
        await loadTasks();
    } catch (error) {
        showMessage(error.message, true);
    }
}

window.addEventListener("DOMContentLoaded", loadTasks);
