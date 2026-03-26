// ============================================
// RUMBA — demo.js
// CRUD de proyectos con localStorage
// ============================================

// --- DATOS INICIALES ---
// Se cargan solo si localStorage está vacío (primera visita)
const DATOS_INICIALES = [
    {
        id: 1,
        nombre: 'Beats para Jerry Di',
        tipo: 'Producción',
        artista: 'Jerry Di',
        entrega: 'Hoy',
        estado: 'En progreso',
        pago: "$1'000.000",
        ep: '',
        urgente: true,
        creadoEn: Date.now() - 86400000
    },
    {
        id: 2,
        nombre: 'Mix Estelar',
        tipo: 'Mix & Master',
        artista: 'MIZ4R',
        entrega: 'Viernes',
        estado: 'En progreso',
        pago: '',
        ep: 'EP Mizar',
        urgente: false,
        creadoEn: Date.now() - 259200000
    },
    {
        id: 3,
        nombre: 'Sesión con Nash',
        tipo: 'Composición',
        artista: 'Nash',
        entrega: '—',
        estado: 'Pendiente',
        pago: '',
        ep: '',
        urgente: false,
        creadoEn: Date.now() - 432000000
    },
    {
        id: 4,
        nombre: "Grabar Voces 'Tus Fotos'",
        tipo: 'Grabación',
        artista: 'Ze7ian',
        entrega: 'Próx. semana',
        estado: 'Pendiente',
        pago: '',
        ep: 'EP Mizar',
        urgente: false,
        creadoEn: Date.now() - 604800000
    },
    {
        id: 5,
        nombre: "Re-Producir 'La Carta'",
        tipo: 'Producción',
        artista: 'Rome & Andy',
        entrega: 'Entregado',
        estado: 'Completado',
        pago: '$800.000',
        ep: '',
        urgente: false,
        creadoEn: Date.now() - 172800000
    }
];

// --- ESTADO DE LA APP ---
let proyectos = [];         // array principal en memoria
let filtroActivo = 'Todos'; // filtro de pill activo
let idEditando = null;      // null = nuevo, número = editar
let idMenuAbierto = null;   // id del proyecto con menú abierto
let nextId = 100;           // ID autoincremental para nuevos proyectos

// --- PERSISTENCIA ---
function cargarDatos() {
    const guardado = localStorage.getItem('rumba_proyectos');
    if (guardado) {
        proyectos = JSON.parse(guardado);
        nextId = Math.max(...proyectos.map(p => p.id)) + 1;
    } else {
        proyectos = DATOS_INICIALES;
        guardarDatos();
    }
}

function guardarDatos() {
    localStorage.setItem('rumba_proyectos', JSON.stringify(proyectos));
}

// --- UTILIDADES ---
// Mapea estado a clase CSS de status-badge
function claseEstado(estado) {
    const mapa = {
        'Pendiente': 'available',
        'En progreso': 'negotiation',
        'En revisión': 'licensed',
        'Completado': 'licensed',
        'Archivado': 'exclusive'
    };
    return mapa[estado] || 'available';
}

// Convierte timestamp a texto relativo (demasiado especifico pero bueno)
function tiempoRelativo(ts) {
    const diff = Date.now() - ts;
    const min = Math.floor(diff / 60000);
    const h = Math.floor(diff / 3600000);
    const d = Math.floor(diff / 86400000);
    const sem = Math.floor(d / 7);
    const mes = Math.floor(d / 30);
    if (min < 2) return 'justo ahora';
    if (h < 1) return `hace ${min} min`;
    if (h < 24) return `hace ${h} hora${h > 1 ? 's' : ''}`;
    if (d === 1) return 'hace 1 día';
    if (d < 7) return `hace ${d} días`;
    if (sem === 1) return 'hace 1 semana';
    if (sem < 4) return `hace ${sem} semanas`;
    if (mes === 1) return 'hace 1 mes';
    return `hace ${mes} meses`;
}

// --- RENDER ---
function renderTabla(lista) {
    const cuerpo = document.getElementById('tabla-body');
    const contador = document.getElementById('contador');

    if (lista.length === 0) {
        cuerpo.innerHTML = `<div class="table-empty">No hay proyectos que mostrar.</div>`;
        contador.textContent = '0 proyectos';
        return;
    }

    contador.textContent = `${lista.length} proyecto${lista.length !== 1 ? 's' : ''}`;

    cuerpo.innerHTML = lista.map(p => {
        // Badge EP
        const badgeEp = p.ep
            ? `<span class="badge-ep">${p.ep}</span>`
            : '';

        // Badge urgente
        const badgeUrgente = p.urgente
            ? `<span class="badge-urgent">Urgente</span>`
            : '';

        // Columna pago
        const colPago = p.pago
            ? `<span class="cell-cash">${p.pago}</span>`
            : `<span class="cell-cash cell-cash--pending">—</span>`;

        return `
      <div class="table-row" data-id="${p.id}">
        <div class="beat-info">
          <button class="play-btn" aria-label="Ver">
            <img src="https://img.icons8.com/material/10/F87171/music--v1.png" alt="" style="width:10px;height:10px;" />
          </button>
          <div>
            <div class="text-bold-sm">${p.nombre}${badgeUrgente}${badgeEp}</div>
            <div class="text-xs-muted beat-sub">${tiempoRelativo(p.creadoEn)}</div>
          </div>
        </div>
        <span class="cell-muted">${p.tipo}</span>
        <span class="cell-muted">${p.artista || '—'}</span>
        <span class="${p.entrega && p.entrega !== '—' ? 'text-bold-sm' : 'cell-muted'}">${p.entrega || '—'}</span>
        <span><span class="status-badge ${claseEstado(p.estado)}">${p.estado}</span></span>
        ${colPago}
      </div>
    `;
    }).join('');

    // Agrega listener de clic derecho / botón ··· a cada fila
    cuerpo.querySelectorAll('.table-row').forEach(fila => {
        fila.addEventListener('contextmenu', (e) => {
            e.preventDefault();
            abrirMenu(e, parseInt(fila.dataset.id));
        });
        // Clic en el botón de play también abre el menú
        fila.querySelector('.play-btn').addEventListener('click', (e) => {
            e.stopPropagation();
            abrirMenu(e, parseInt(fila.dataset.id));
        });
    });
}

// Aplica filtro activo y búsqueda, luego renderiza
function renderFiltrado() {
    const busqueda = document.getElementById('input-buscar').value.toLowerCase();
    let lista = proyectos;

    if (filtroActivo !== 'Todos') {
        if (filtroActivo === 'Urgentes') {
            lista = lista.filter(p => p.urgente);
        } else {
            lista = lista.filter(p => p.tipo === filtroActivo);
        }
    }

    if (busqueda) {
        lista = lista.filter(p =>
            p.nombre.toLowerCase().includes(busqueda) ||
            (p.artista && p.artista.toLowerCase().includes(busqueda)) ||
            p.tipo.toLowerCase().includes(busqueda)
        );
    }

    renderTabla(lista);
}

// --- FILTROS ---
function filtrar(tipo, btn) {
    filtroActivo = tipo;
    document.querySelectorAll('.pill').forEach(p => p.classList.remove('pill-active'));
    btn.classList.add('pill-active');
    renderFiltrado();
}

// --- MODAL ---
function abrirModal(id = null) {
    cerrarMenu();
    idEditando = id;

    const overlay = document.getElementById('modal-overlay');
    const titulo = document.getElementById('modal-titulo');

    if (id !== null) {
        // Modo editar — precarga los campos
        const p = proyectos.find(x => x.id === id);
        titulo.textContent = 'Editar proyecto';
        document.getElementById('campo-nombre').value = p.nombre;
        document.getElementById('campo-tipo').value = p.tipo;
        document.getElementById('campo-estado').value = p.estado;
        document.getElementById('campo-artista').value = p.artista || '';
        document.getElementById('campo-entrega').value = p.entrega || '';
        document.getElementById('campo-pago').value = p.pago || '';
        document.getElementById('campo-ep').value = p.ep || '';
    } else {
        // Modo nuevo — limpia campos
        // titulo.textContent = 'Nueva Rumba';
        document.getElementById('campo-nombre').value = '';
        document.getElementById('campo-tipo').value = 'Producción';
        document.getElementById('campo-estado').value = 'Pendiente';
        document.getElementById('campo-artista').value = '';
        document.getElementById('campo-entrega').value = '';
        document.getElementById('campo-pago').value = '';
        document.getElementById('campo-ep').value = '';
    }

    overlay.classList.add('active');
    document.getElementById('campo-nombre').focus();
}

function cerrarModal() {
    document.getElementById('modal-overlay').classList.remove('active');
    idEditando = null;
}

function guardarProyecto() {
    const nombre = document.getElementById('campo-nombre').value.trim();
    if (!nombre) {
        document.getElementById('campo-nombre').focus();
        document.getElementById('campo-nombre').style.borderColor = 'var(--red-500)';
        return;
    }
    document.getElementById('campo-nombre').style.borderColor = '';

    const datos = {
        nombre,
        tipo: document.getElementById('campo-tipo').value,
        estado: document.getElementById('campo-estado').value,
        artista: document.getElementById('campo-artista').value.trim(),
        entrega: document.getElementById('campo-entrega').value.trim() || '—',
        pago: document.getElementById('campo-pago').value.trim(),
        ep: document.getElementById('campo-ep').value.trim(),
        urgente: false
    };

    if (idEditando !== null) {
        // Actualizar existente
        const idx = proyectos.findIndex(p => p.id === idEditando);
        proyectos[idx] = { ...proyectos[idx], ...datos };
    } else {
        // Crear nuevo
        proyectos.unshift({
            id: nextId++,
            ...datos,
            creadoEn: Date.now()
        });
    }

    guardarDatos();
    cerrarModal();
    renderFiltrado();
}

// --- MENÚ CONTEXTUAL ---
function abrirMenu(e, id) {
    cerrarMenu();
    idMenuAbierto = id;
    const menu = document.getElementById('row-menu');
    menu.style.top = `${e.clientY + window.scrollY}px`;
    menu.style.left = `${Math.min(e.clientX, window.innerWidth - 160)}px`;
    menu.style.position = 'absolute';
    menu.classList.add('active');
}

function cerrarMenu() {
    document.getElementById('row-menu').classList.remove('active');
    idMenuAbierto = null;
}

function eliminarProyecto(id) {
    if (!confirm('¿Eliminar este proyecto?')) return;
    proyectos = proyectos.filter(p => p.id !== id);
    guardarDatos();
    renderFiltrado();
}

// --- EVENTOS ---
document.addEventListener('DOMContentLoaded', () => {
    cargarDatos();
    renderFiltrado();

    // Botón nuevo proyecto
    document.getElementById('btn-nuevo').addEventListener('click', () => abrirModal());

    // Cerrar modal
    document.getElementById('modal-cerrar').addEventListener('click', cerrarModal);
    document.getElementById('btn-cancelar').addEventListener('click', cerrarModal);
    document.getElementById('modal-overlay').addEventListener('click', (e) => {
        if (e.target === document.getElementById('modal-overlay')) cerrarModal();
    });

    // Guardar desde modal
    document.getElementById('btn-guardar').addEventListener('click', guardarProyecto);

    // Enter en campo nombre guarda también
    document.getElementById('campo-nombre').addEventListener('keydown', (e) => {
        if (e.key === 'Enter') guardarProyecto();
    });

    // Menú contextual — editar / eliminar
    document.getElementById('menu-editar').addEventListener('click', () => {
        const id = idMenuAbierto;
        cerrarMenu();
        abrirModal(id);
    });

    document.getElementById('menu-eliminar').addEventListener('click', () => {
        const id = idMenuAbierto;
        cerrarMenu();
        eliminarProyecto(id);
    });

    // Cerrar menú al clicar fuera
    document.addEventListener('click', (e) => {
        if (!document.getElementById('row-menu').contains(e.target)) {
            cerrarMenu();
        }
    });

    // Búsqueda en tiempo real
    document.getElementById('input-buscar').addEventListener('input', renderFiltrado);

    // Ctrl+N abre modal desde cualquier parte
    document.addEventListener('keydown', (e) => {
        if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
            e.preventDefault();
            abrirModal();
        }
        // Escape cierra modal o menú
        if (e.key === 'Escape') {
            cerrarModal();
            cerrarMenu();
        }
    });
});
