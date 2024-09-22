
const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);


const repoName = document.getElementById("repoName");
const repoVisibility = document.getElementById("repoVisibility");
const repoTags = document.getElementById("repoTags");
const branchSelect = document.getElementById("branchSelect");
const branchNameInput = document.getElementById("branchNameInput");
const controlDiv = document.getElementById("controlDiv");
const multParamHeader = new Headers().append("Content-Type", "application/x-www-form-urlencoded");
const commentsContainer = document.getElementsByClassName("comment-container")[0];
const commentBtnContainer = document.getElementById("commentBtnContainer");
const confirmBtnContainer = document.getElementById("confirmBtnContainer");

let repository = null;
let selectedComment = null;
let commentsBase = null;
let comments = null;


document.addEventListener('DOMContentLoaded', function () {
    loadRepo(urlParams.get("id"));    
}, false);


//Functions

async function loadRepo(id) {
    const url = `https://localhost:7269/api/repository/public/${id}`;
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }

        repository = await response.json();

        displayRepository(repository);

        checkUserPermission(repository);

    } catch (error) {
        console.error(error.message);
    }
}

function checkUserPermission(repository) {
    let userId = sessionStorage.getItem("_UserId");
    console.log(userId);
    console.log(repository.userId);
    if (repository == null) return;


    if (repository.userId == userId) {
        controlDiv.classList.remove("invisible");
    }
}


function displayRepository(repoObject) {
    repoName.innerHTML = repoObject.name;
    repoVisibility.innerHTML += repoObject.visibility;
    repoTags.innerHTML += repoObject.tags;

    let branches = repoObject.branches;


    for (let branch of branches) {
        branchSelect.innerHTML += `<option value="${branch.name}">${branch.name}</option>`;
        branchSelect.lastChild.addEventListener("change", displayBranch(branch));
    }
}


document.getElementById("modifyBtn").addEventListener("click", (e) => {
    window.location.href = `/ModifyRepository?id=${repository.id}&v=${repository.visibility}`;;
});

document.getElementById("deleteBtn").addEventListener("click", async (e) => {
    const url = `https://localhost:7269/api/repository/${repository.id}`;
    try {
        const response = await fetch(url, { method: "DELETE" });
        if (!response.ok) {
            alert("Unexpected error");
            return;
        }

        window.location.pathname = "/UserPage";

    } catch (error) {
        console.error(error.message);
    }
});

document.getElementById("createBranchBtn").addEventListener("click", async (e) => {
    e.preventDefault();
    const url = `https://localhost:7269/api/repository/${repository.id}/branch`;
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    let selectedBranchCommit = null;

    for (let branch of repository.branches) {
        if (branch.name == branchSelect.value) {
            selectedBranchCommit = branch.latestCommit;
            break;
        }
    }

    if (selectedBranchCommit == null) {
        alert("No se puede crear un branch a partir de un branch sin commits");
        return;
    }

    //Name
    let name = branchNameInput.value;
    name.trim();
    if (name == "") {
        return;
    } else if (!isAlphanumeric(name)) {
        alert("El nombre del repositorio solo debe contener caracteres alfanumericos");
    }
    name = name.replaceAll(" ", "-");

    console.log(selectedBranchCommit)
    try {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({ name: name, latestCommit: selectedBranchCommit }),
            headers: myHeaders
        });

        if (!response.ok) {
            let obj = await response.text();
            alert(obj)
        }
        location.reload()

    } catch (error) {
        console.error(error.message);
        alert("Ocurrió un error inesperado");
    }
});

document.getElementById("deleteBranchBtn").addEventListener("click", async (e) => {
    if (branchSelect.value == "Master") { alert("El Branch Master no puede ser borrado"); return; }
    const url = `https://localhost:7269/api/repository/${repository.id}/branch/${branchSelect.value}`;
    try {
        const response = await fetch(url, { method: "DELETE" });
        if (!response.ok) {
            alert("Unexpected error");
            return;
        }
    } catch (error) {
        console.error(error.message);
    }
});

function isAlphanumeric(text) {
    const alphanumericRegex = /^[a-z0-9 -]+$/i;
    return alphanumericRegex.test(text);
}



function displayBranch(branch){
    $("#branchDiv").append(`<h5>${branch.latestCommit}: poner datos del commit o algo así</h5>`);
}


// Comments Functions

async function loadRepoComments(repoId) {
    const commentsUrl = `https://localhost:7269/api/comment/GetByRepoId/${repoId}`;

    try {
        const commResponse = await fetch(commentsUrl);

        if (!commResponse.ok) {
            throw new Error(`Response status: ${commResponse.status}`);
        }

        comments = await commResponse.json();

        if (Array.isArray(comments)) {
            displayComments(comments);
        }

    } catch (error) {
        console.error(error.message);
    }
}

async function insertComment(commentMessage) {
    const commentsUrl = `https://localhost:7269/api/comment/InsertComment`;

    try {
        const commResponse = await fetch(commentsUrl, {
            method: "POST",
            body: JSON.stringify({
                user: "user", // Change to userId
                message: commentMessage,
                repoId: "string", // Change to repoId
                subcomments: []
            }),
            headers: { "Content-Type": "application/json" }
        });

        if (!commResponse.ok) {
            throw new Error(`Response status: ${commResponse.status}`);
        }

    } catch (error) {
        console.error(error.message);
    }
}

async function deleteComment(commentId) {
    const commentsUrl = `https://localhost:7269/api/comment/DeleteComment/${commentId}`;

    try {
        const commResponse = await fetch(commentsUrl, { method: "DELETE" });

        if (!commResponse.ok) {
            throw new Error(`Response status: ${commResponse.status}`);
        }

        loadRepoComments("string"); // Change to repoId

    } catch (error) {
        console.error(error.message);
    }
}

async function insertSubcomment(commentId, commentMessage) {
    const commentsUrl = `https://localhost:7269/api/comment/${commentId}/InsertSubcomment`;

    try {
        const commResponse = await fetch(commentsUrl, {
            method: "POST",
            body: JSON.stringify({
                user: "user", // Change to userId
                message: commentMessage,
            }),
            headers: { "Content-Type": "application/json" }
        });

        if (!commResponse.ok) {
            throw new Error(`Response status: ${commResponse.status}`);
        }

    } catch (error) {
        console.error(error.message);
    }
}

async function deleteSubcomment(commentId, commentMessage) {
    const commentsUrl = `https://localhost:7269/api/comment/${commentId}/DeleteSubcomment`;

    try {
        const commResponse = await fetch(commentsUrl, {
            method: "DELETE",
            body: JSON.stringify({
                user: "user", // Change to userId
                message: commentMessage,
            }),
            headers: { "Content-Type": "application/json" }
        });

        if (!commResponse.ok) {
            throw new Error(`Response status: ${commResponse.status}`);
        }

    } catch (error) {
        console.error(error.message);
    }
}


function displayComments(commentsList) {
    let containerHTML = "";
    commentsList.forEach(comment => {
        containerHTML += commentFormat(comment);
    })

    commentsContainer.innerHTML = containerHTML

    setActions();

    commentsBase = commentsContainer.innerHTML;
}



function commentFormat(commentObj) {
    let commentHTML = `<div class="card mt-3 shadow-lg" id="${commentObj.id}">
                                    <div class="comment-header bg-primary text-white p-3">
                                        <div>
                                            <span class="card-title mb-0">${commentObj.user}</span>
                                            <span class="comment-date">${getDate(commentObj)}</span>
                                        </div>
                                        <div class="button-container">
                                            <button class="delete-btn">Delete</button>
                                            <button class="edit-btn">Edit</button>
                                        </div>
                                    </div>
                                    <div class="card-body">
                                        <div class="comment-message">${commentObj.message}</div>
                                        <div class="button-container">
                                            <button class="response-btn">Responder</button>
                                        </div>
                                        <button class="show-subcomment-btn">Subcomentarios(${commentObj.subcomments.length}) ></button>
                                        <div class="subcomment-container" style="display: none;" id="${commentObj.id}[subcomments]">`;

    let index = 0;
    commentObj.subcomments.forEach(subcomment => {
        commentHTML += subcommentFormat(subcomment, commentObj.id, index);
        index += 1;
    });

    commentHTML += `</div>
                                </div>
                            </div>`;

    return commentHTML;
}

function subcommentFormat(subcommentObj, commentId, index) {
    let subcommentHTML = `<div class="subcomment" id="${commentId}[subcomments][${index}]">
                                        <div class="comment-header bg-secondary text-white p-2">
                                            <div>
                                                <span class="card-title mb-0">${subcommentObj.user}</span>
                                                <span class="comment-date">${getDate(subcommentObj)}</span>
                                            </div>
                                            <div class="button-container">
                                                <button class="delete-btn subcomment-btn">Eliminar</button>
                                                <button class="edit-btn subcomment-btn">Editar</button>
                                            </div>
                                        </div>
                                        <div class="comment-message p-2">${subcommentObj.message}</div>
                                    </div>`;

    return subcommentHTML;
}

function getDate(comment) {
    let date = formatDate(comment.creationDate);

    if (comment.creationDate != comment.lastDate) {
        date += ` (Edited ${formatDate(comment.lastDate)})`;
    }

    return date;
}

function formatDate(date) {
    const options = date.split(/[-T.Z]/);
    let formatedDate = `${options[2]}/${options[1]}/${options[0]} ${options[3]}`;
    return formatedDate;
}



function commentBtnAction() {
    document.getElementById("commentBtn").addEventListener("click", function () {
        resetCommentsView();
        commentsContainer.innerHTML = commentsBase + `<input class="input-comment" type="text" id="writeComment" placeholder="Write your comment here"></input>`;
        confirmBtnContainer.innerHTML = `<button class="response-btn" id="acceptBtn">Aceptar</button>
                                                 <button class="response-btn" id="cancelBtn">Cancelar</button>`;

        const inputComment = document.getElementById("writeComment");
        inputComment.scrollIntoView({ behavior: 'smooth' });

        document.getElementById("acceptBtn").addEventListener("click", async function () {
            let message = inputComment.value;

            if (!isEmptyOrSpace(message)) {
                await insertComment(message);
            }

            updateCommentsView();
        });

        document.getElementById("cancelBtn").addEventListener("click", function () {
            updateCommentsView();
        });
    });
}

function setDeleteAction() {
    const deleteBtns = document.getElementsByClassName("delete-btn");
    const commentDeleteBtns = Array.from(deleteBtns).filter(element =>
        !element.classList.contains("subcomment-btn")
    );
    const subcommentDeleteBtns = Array.from(deleteBtns).filter(element =>
        element.classList.contains("subcomment-btn")
    );

    commentDeleteBtns.forEach(deleteBtn => {
        deleteBtn.addEventListener("click", async (e) => {
            const commentId = e.currentTarget.parentNode.parentNode.parentNode.id;
            await deleteComment(commentId);
            loadRepoComments("string"); // Change to repoId
        });
    });

    subcommentDeleteBtns.forEach(deleteBtn => {
        deleteBtn.addEventListener("click", async (e) => {
            const subcommentId = e.currentTarget.parentNode.parentNode.parentNode.id;
            const message = document.getElementById(subcommentId).getElementsByClassName("comment-message")[0].textContent;
            await deleteSubcomment(getCommentId(subcommentId), message);
            loadRepoComments("string"); // Change to repoId
        });
    });
}

function setDisplaySubcomments() {
    const subcomBtns = document.getElementsByClassName("show-subcomment-btn");

    Array.from(subcomBtns).forEach(subcomentBtn => {
        subcomentBtn.addEventListener("click", (e) => {
            const subcomContainer = e.currentTarget.parentNode.getElementsByClassName("subcomment-container")[0];

            if (subcomContainer.style.display == 'none') {
                subcomContainer.style.display = `block`;
                subcomentBtn.textContent = subcomentBtn.textContent.slice(0, -1) + "∧"
            } else {
                subcomContainer.style.display = 'none';
                subcomentBtn.textContent = subcomentBtn.textContent.slice(0, -1) + ">"
            }
        });
    });
}

function setRespondAction() {
    const respondBtns = document.getElementsByClassName("response-btn");

    Array.from(respondBtns).forEach(respondBtn => {
        respondBtn.addEventListener("click", respond);
    });
}

function respond(event) {
    resetCommentsView();

    const commentId = event.currentTarget.parentNode.parentNode.parentNode.id;
    const subcommentsList = document.getElementById(`${commentId}[subcomments]`);

    subcommentsList.innerHTML += `<input class="input-comment" type="text" id="writeComment" placeholder="Write your comment here"></input>
                                          <div class="confirm-container" id="confirmSubcommentContainer">
                                              <button class="response-btn" id="acceptBtn">Aceptar</button>
                                              <button class="response-btn" id="cancelBtn">Cancelar</button>
                                          </div>`;

    const inputComment = document.getElementById("writeComment");
    inputComment.scrollIntoView({ behavior: 'smooth' });

    document.getElementById("acceptBtn").addEventListener("click", async function () {
        let message = inputComment.value;

        if (!isEmptyOrSpace(message)) {
            await insertSubcomment(commentId, message);
        }

        updateCommentsView();
    });

    document.getElementById("cancelBtn").addEventListener("click", function () {
        updateCommentsView();
    });
}



function isEmptyOrSpace(string) {
    return string.trim().length === 0;
}

function setActions() {
    commentBtnAction();
    setRespondAction();
    setDeleteAction();
    setDisplaySubcomments()
}

function resetCommentsView() {
    commentsContainer.innerHTML = commentsBase;

    setActions();
}

async function updateCommentsView() {
    await loadRepoComments("string"); // Change to repoId
    confirmBtnContainer.innerHTML = "";
}

function getCommentId(subcommentId) {
    const regex = /^([^\[]+)\[subcomments\]\[(\d+)\]$/;
    return subcommentId.match(regex)[1];
}

loadRepoComments("string");// Change to repoId