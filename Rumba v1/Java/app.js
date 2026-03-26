// ============================================
// SISTEMA DE AUTENTICACION - RUMBA
// ============================================

function showMessage(text, type) {
  const messageDiv = document.getElementById("message");
  if (messageDiv) {
    messageDiv.textContent = text;
    messageDiv.className = "message " + type;


    setTimeout(function () {
      if (messageDiv) {
        messageDiv.textContent = "";
        messageDiv.className = "message";
      }
    }, 3000);
  }
}


function register() {

  const username = document.getElementById("username").value.trim();
  const password = document.getElementById("password").value;


  if (!username || !password) {
    showMessage("Completa todos los campos", "error");
    return;
  }


  if (password.length < 4) {
    showMessage("La contraseña debe tener al menos 4 caracteres", "error");
    return;
  }


  let users = JSON.parse(localStorage.getItem("users")) || [];


  const userExists = users.find(function (user) {
    return user.username === username;
  });

  if (userExists) {
    showMessage("Este usuario ya existe", "error");
    return;
  }


  const newUser = {
    username: username,
    password: password,
    createdAt: new Date().toISOString()
  };


  users.push(newUser);


  localStorage.setItem("users", JSON.stringify(users));


  showMessage("Usuario creado correctamente", "success");


  document.getElementById("username").value = "";
  document.getElementById("password").value = "";
}


function login() {
  const username = document.getElementById("username").value.trim();
  const password = document.getElementById("password").value;


  if (!username || !password) {
    showMessage("Ingresa usuario y contraseña", "error");
    return;
  }


  let users = JSON.parse(localStorage.getItem("users")) || [];


  const validUser = users.find(function (user) {
    return user.username === username && user.password === password;
  });


  if (validUser) {

    localStorage.setItem("isLoggedIn", "true");
    localStorage.setItem("currentUser", username);

    showMessage("Bienvenido " + username, "success");


    setTimeout(function () {
      window.location.href = "index.html";
    }, 1000);

  } else {
    showMessage("Usuario o contraseña incorrectos", "error");
  }
}


function checkAuth() {
  const isLoggedIn = localStorage.getItem("isLoggedIn");

  if (isLoggedIn !== "true") {
    window.location.href = "login.html";
  }
}


function logout() {
  localStorage.removeItem("isLoggedIn");
  localStorage.removeItem("currentUser");
  window.location.href = "login.html";
}


function updateUserDisplay() {
  const currentUser = localStorage.getItem("currentUser");
  const userDisplayElements = document.querySelectorAll(".user-display");

  for (var i = 0; i < userDisplayElements.length; i++) {
    userDisplayElements[i].textContent = currentUser || "Invitado";
  }
}


function createDefaultAdmin() {
  let users = JSON.parse(localStorage.getItem("users")) || [];

  const adminExists = users.find(function (user) {
    return user.username === "admin";
  });

  if (!adminExists) {
    const adminUser = {
      username: "admin",
      password: "admin123",
      createdAt: new Date().toISOString()
    };

    users.push(adminUser);
    localStorage.setItem("users", JSON.stringify(users));

    console.log("Usuario administrador creado");
    console.log("Usuario: admin");
    console.log("Contraseña: admin123");
  }
}


function listAllUsers() {
  let users = JSON.parse(localStorage.getItem("users")) || [];
  console.log("Usuarios registrados:", users);
  return users;
}


function resetAllData() {
  var confirmar = confirm("¿Eliminar todos los usuarios y datos?");
  if (confirmar) {
    localStorage.removeItem("users");
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("currentUser");
    console.log("Todos los datos han sido eliminados");
    location.reload();
  }
}


createDefaultAdmin();


var path = window.location.pathname;
if (path.includes("index.html") || path === "/" || path.endsWith("/")) {
  checkAuth();
  updateUserDisplay();
}


window.register = register;
window.login = login;
window.logout = logout;
window.checkAuth = checkAuth;
window.updateUserDisplay = updateUserDisplay;
window.listAllUsers = listAllUsers;
window.resetAllData = resetAllData;