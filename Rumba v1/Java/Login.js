// ===============================
// INICIALIZAR USUARIOS
// ===============================

const container = document.getElementById("container");
const registerBtn = document.getElementById("register");
const loginBtn = document.getElementById("login");

registerBtn.addEventListener("click", () => {
  container.classList.add("active");
});

loginBtn.addEventListener("click", () => {
  container.classList.remove("active");
});

if (!localStorage.getItem("usuarios")) {
  const usuarios = [
    {
      nombre: "Admin",
      email: "admin@rumba.com",
      password: "1234"
    }
  ];
  localStorage.setItem("usuarios", JSON.stringify(usuarios));
}

// ===============================
// ELEMENTOS
// ===============================
const btnRegistrar = document.getElementById("btnRegistrar");
const btnLogin = document.getElementById("btnLogin");

// ===============================
// REGISTRO
// ===============================
btnRegistrar.addEventListener("click", () => {
  const nombre = document.getElementById("regNombre").value;
  const email = document.getElementById("regEmail").value;
  const password = document.getElementById("regPassword").value;

  if (!nombre || !email || !password) {
    alert("Completa todos los campos");
    return;
  }

  let usuarios = JSON.parse(localStorage.getItem("usuarios"));

  // Verificar si ya existe
  const existe = usuarios.find(u => u.email === email);

  if (existe) {
    alert("El usuario ya existe");
    return;
  }

  usuarios.push({ nombre, email, password });

  localStorage.setItem("usuarios", JSON.stringify(usuarios));

  alert("Usuario registrado correctamente");
});

// ===============================
// LOGIN
// ===============================
btnLogin.addEventListener("click", () => {
  const email = document.getElementById("logEmail").value;
  const password = document.getElementById("logPassword").value;

  let usuarios = JSON.parse(localStorage.getItem("usuarios"));

  const usuario = usuarios.find(
    u => u.email === email && u.password === password
  );

  if (!usuario) {
    alert("Credenciales incorrectas");
    return;
  }

  // Guardar sesión
  localStorage.setItem("usuarioActivo", JSON.stringify(usuario));

  // Redirigir
  window.location.href = "index.html";
});