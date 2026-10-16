import os
import requests
from urllib.parse import quote
from functools import wraps
from flask import Flask, render_template, request, redirect, url_for, flash, session

app = Flask(__name__)
app.secret_key = os.getenv("SECRET_KEY", "mira-secret-key-prod-local")

# URL base de la API REST de MIRA
API_URL = os.getenv("API_URL", "http://localhost:8081/api").rstrip("/")

def api_call(method, path, json_data=None, params=None):
    """Función auxiliar para realizar llamadas seguras inyectando el token JWT."""
    url = f"{API_URL}/{path.lstrip('/')}"
    headers = {"Content-Type": "application/json"}
    
    # Inyectar el token JWT de la sesión activa si existe
    token = session.get("jwt_token")
    if token:
        headers["Authorization"] = f"Bearer {token}"

    try:
        response = requests.request(
            method=method,
            url=url,
            json=json_data,
            params=params,
            headers=headers,
            timeout=8
        )
        return response
    except requests.exceptions.RequestException:
        return None

# =============================================================================
# Control de Acceso y Decoradores RBAC
# =============================================================================
def login_required(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        if "jwt_token" not in session:
            flash("Debe iniciar sesión para acceder al sistema.", "warning")
            return redirect(url_for("login", next=request.url))
        return f(*args, **kwargs)
    return decorated_function

def roles_required(*roles):
    def decorator(f):
        @wraps(f)
        def decorated_function(*args, **kwargs):
            if "jwt_token" not in session:
                flash("Debe iniciar sesión para acceder.", "warning")
                return redirect(url_for("login"))
            user_roles = session.get("user", {}).get("roles", [])
            if not any(r in user_roles for r in roles):
                flash("No tiene permisos suficientes para realizar esta acción.", "danger")
                return redirect(url_for("index"))
            return f(*args, **kwargs)
        return decorated_function
    return decorator

@app.context_processor
def inject_user():
    """Inyecta el usuario y su estado de autenticación a todas las plantillas Jinja2."""
    return {
        "current_user": session.get("user"),
        "is_authenticated": "jwt_token" in session
    }

# =============================================================================
# Autenticación y Cierre de Sesión
# =============================================================================
@app.route("/login", methods=["GET", "POST"])
def login():
    if "jwt_token" in session:
        return redirect(url_for("index"))

    if request.method == "POST":
        username = request.form.get("username", "").strip()
        password = request.form.get("password", "").strip()

        resp = api_call("POST", "Auth/login", {"username": username, "password": password})
        if resp and resp.status_code == 200:
            data = resp.json()
            session["jwt_token"] = data.get("token")
            session["user"] = data.get("usuario")
            flash(f"¡Bienvenido(a), {session['user'].get('nombreCompleto') or username}!", "success")
            next_url = request.args.get("next")
            return redirect(next_url or url_for("index"))

        error_msg = resp.json().get("detalle", "Credenciales inválidas.") if resp else "Error de conexión con el backend."
        flash(error_msg, "danger")

    return render_template("login.html")

@app.route("/logout")
def logout():
    session.clear()
    flash("Sesión cerrada correctamente.", "info")
    return redirect(url_for("login"))

# =============================================================================
# Dashboard Principal
# =============================================================================
@app.route("/")
@login_required
def index():
    return render_template("index.html")

# =============================================================================
# Gestión de Usuarios (Exclusivo Administrador)
# =============================================================================
@app.route("/usuarios")
@roles_required("Administrador")
def usuarios_list():
    resp = api_call("GET", "Usuario")
    usuarios = resp.json().get("datos", []) if resp and resp.status_code == 200 else []
    return render_template("modulos/usuario/list.html", usuarios=usuarios)

@app.route("/usuarios/nuevo")
@roles_required("Administrador")
def usuarios_new():
    return render_template("modulos/usuario/form.html")

@app.route("/usuarios/crear", methods=["POST"])
@roles_required("Administrador")
def usuarios_create():
    rol_id = int(request.form.get("rol_id", 3))
    payload = {
        "username": request.form.get("username", "").strip(),
        "password": request.form.get("password", "").strip(),
        "email": request.form.get("email", "").strip(),
        "nombreCompleto": request.form.get("nombre_completo", "").strip(),
        "rolesIds": [rol_id]
    }
    resp = api_call("POST", "Usuario", payload)
    if resp and resp.status_code == 201:
        flash("Usuario registrado exitosamente.", "success")
        return redirect(url_for("usuarios_list"))
    
    error_msg = resp.json().get("mensaje", "Error al crear usuario.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/usuario/form.html", item=payload)

@app.route("/usuarios/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def usuarios_delete(id):
    resp = api_call("DELETE", f"Usuario/{id}")
    if resp and resp.status_code == 200:
        flash("Usuario desactivado exitosamente.", "success")
    else:
        flash("No se pudo desactivar el usuario.", "danger")
    return redirect(url_for("usuarios_list"))

# =============================================================================
# Módulo 1: Área de Conocimiento
# =============================================================================
@app.route("/area_conocimiento")
@login_required
def area_conocimiento_list():
    resp = api_call("GET", "area_conocimiento")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/area_conocimiento/list.html", items=items)

@app.route("/area_conocimiento/nuevo")
@roles_required("Administrador", "Editor")
def area_conocimiento_new():
    return render_template("modulos/area_conocimiento/form.html", item=None)

@app.route("/area_conocimiento/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def area_conocimiento_create():
    payload = {
        "granArea": request.form.get("gran_area", "").strip(),
        "area": request.form.get("area", "").strip(),
        "disciplina": request.form.get("disciplina", "").strip()
    }
    resp = api_call("POST", "area_conocimiento", payload)
    if resp and resp.status_code == 201:
        flash("Área de conocimiento creada exitosamente.", "success")
        return redirect(url_for("area_conocimiento_list"))
    error_msg = resp.json().get("mensaje", "Error al crear.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/area_conocimiento/form.html", item=payload)

@app.route("/area_conocimiento/editar/<int:id>")
@roles_required("Administrador", "Editor")
def area_conocimiento_edit(id):
    resp = api_call("GET", f"area_conocimiento/{id}")
    if resp and resp.status_code == 200:
        return render_template("modulos/area_conocimiento/form.html", item=resp.json())
    flash("Registro no encontrado o inactivo.", "warning")
    return redirect(url_for("area_conocimiento_list"))

@app.route("/area_conocimiento/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def area_conocimiento_update(id):
    payload = {
        "id": id,
        "granArea": request.form.get("gran_area", "").strip(),
        "area": request.form.get("area", "").strip(),
        "disciplina": request.form.get("disciplina", "").strip()
    }
    resp = api_call("PUT", f"area_conocimiento/{id}", payload)
    if resp and resp.status_code == 200:
        flash("Área de conocimiento actualizada exitosamente.", "success")
        return redirect(url_for("area_conocimiento_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/area_conocimiento/form.html", item=payload)

@app.route("/area_conocimiento/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def area_conocimiento_delete(id):
    resp = api_call("DELETE", f"area_conocimiento/{id}")
    if resp and resp.status_code == 200:
        flash("Área de conocimiento eliminada lógicamente.", "success")
    else:
        flash("No se pudo eliminar el registro.", "danger")
    return redirect(url_for("area_conocimiento_list"))

# =============================================================================
# Módulo 2: Objetivo de Desarrollo Sostenible (ODS)
# =============================================================================
@app.route("/objetivo_desarrollo_sostenible")
@login_required
def ods_list():
    resp = api_call("GET", "objetivo_desarrollo_sostenible")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/objetivo_desarrollo_sostenible/list.html", items=items)

@app.route("/objetivo_desarrollo_sostenible/nuevo")
@roles_required("Administrador", "Editor")
def ods_new():
    return render_template("modulos/objetivo_desarrollo_sostenible/form.html", item=None)

@app.route("/objetivo_desarrollo_sostenible/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def ods_create():
    payload = {
        "nombre": request.form.get("nombre", "").strip(),
        "categoria": request.form.get("categoria", "").strip()
    }
    resp = api_call("POST", "objetivo_desarrollo_sostenible", payload)
    if resp and resp.status_code == 201:
        flash("ODS creado exitosamente.", "success")
        return redirect(url_for("ods_list"))
    error_msg = resp.json().get("mensaje", "Error al crear.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/objetivo_desarrollo_sostenible/form.html", item=payload)

@app.route("/objetivo_desarrollo_sostenible/editar/<int:id>")
@roles_required("Administrador", "Editor")
def ods_edit(id):
    resp = api_call("GET", f"objetivo_desarrollo_sostenible/{id}")
    if resp and resp.status_code == 200:
        return render_template("modulos/objetivo_desarrollo_sostenible/form.html", item=resp.json())
    flash("Registro no encontrado o inactivo.", "warning")
    return redirect(url_for("ods_list"))

@app.route("/objetivo_desarrollo_sostenible/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def ods_update(id):
    payload = {
        "id": id,
        "nombre": request.form.get("nombre", "").strip(),
        "categoria": request.form.get("categoria", "").strip()
    }
    resp = api_call("PUT", f"objetivo_desarrollo_sostenible/{id}", payload)
    if resp and resp.status_code == 200:
        flash("ODS actualizado exitosamente.", "success")
        return redirect(url_for("ods_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/objetivo_desarrollo_sostenible/form.html", item=payload)

@app.route("/objetivo_desarrollo_sostenible/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def ods_delete(id):
    resp = api_call("DELETE", f"objetivo_desarrollo_sostenible/{id}")
    if resp and resp.status_code == 200:
        flash("ODS eliminado lógicamente.", "success")
    else:
        flash("No se pudo eliminar el registro.", "danger")
    return redirect(url_for("ods_list"))

# =============================================================================
# Módulo 3: Área de Aplicación
# =============================================================================
@app.route("/area_aplicacion")
@login_required
def area_aplicacion_list():
    resp = api_call("GET", "area_aplicacion")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/area_aplicacion/list.html", items=items)

@app.route("/area_aplicacion/nuevo")
@roles_required("Administrador", "Editor")
def area_aplicacion_new():
    return render_template("modulos/area_aplicacion/form.html", item=None)

@app.route("/area_aplicacion/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def area_aplicacion_create():
    payload = {
        "nombre": request.form.get("nombre", "").strip()
    }
    resp = api_call("POST", "area_aplicacion", payload)
    if resp and resp.status_code == 201:
        flash("Área de aplicación creada exitosamente.", "success")
        return redirect(url_for("area_aplicacion_list"))
    error_msg = resp.json().get("mensaje", "Error al crear.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/area_aplicacion/form.html", item=payload)

@app.route("/area_aplicacion/editar/<int:id>")
@roles_required("Administrador", "Editor")
def area_aplicacion_edit(id):
    resp = api_call("GET", f"area_aplicacion/{id}")
    if resp and resp.status_code == 200:
        return render_template("modulos/area_aplicacion/form.html", item=resp.json())
    flash("Registro no encontrado o inactivo.", "warning")
    return redirect(url_for("area_aplicacion_list"))

@app.route("/area_aplicacion/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def area_aplicacion_update(id):
    payload = {
        "id": id,
        "nombre": request.form.get("nombre", "").strip()
    }
    resp = api_call("PUT", f"area_aplicacion/{id}", payload)
    if resp and resp.status_code == 200:
        flash("Área de aplicación actualizada exitosamente.", "success")
        return redirect(url_for("area_aplicacion_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/area_aplicacion/form.html", item=payload)

@app.route("/area_aplicacion/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def area_aplicacion_delete(id):
    resp = api_call("DELETE", f"area_aplicacion/{id}")
    if resp and resp.status_code == 200:
        flash("Área de aplicación eliminada lógicamente.", "success")
    else:
        flash("No se pudo eliminar el registro.", "danger")
    return redirect(url_for("area_aplicacion_list"))

# =============================================================================
# Módulo 4: Término Clave (PK string)
# =============================================================================
@app.route("/termino_clave")
@login_required
def termino_clave_list():
    resp = api_call("GET", "termino_clave")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/termino_clave/list.html", items=items)

@app.route("/termino_clave/nuevo")
@roles_required("Administrador", "Editor")
def termino_clave_new():
    return render_template("modulos/termino_clave/form.html", item=None, is_new=True)

@app.route("/termino_clave/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def termino_clave_create():
    payload = {
        "termino": request.form.get("termino", "").strip(),
        "terminoIngles": request.form.get("termino_ingles", "").strip() or None
    }
    resp = api_call("POST", "termino_clave", payload)
    if resp and resp.status_code == 201:
        flash("Término clave creado exitosamente.", "success")
        return redirect(url_for("termino_clave_list"))
    error_msg = resp.json().get("mensaje", "Error al crear.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/termino_clave/form.html", item=payload, is_new=True)

@app.route("/termino_clave/editar/<path:termino>")
@roles_required("Administrador", "Editor")
def termino_clave_edit(termino):
    encoded = quote(termino, safe="")
    resp = api_call("GET", f"termino_clave/{encoded}")
    if resp and resp.status_code == 200:
        return render_template("modulos/termino_clave/form.html", item=resp.json(), is_new=False)
    flash("Registro no encontrado o inactivo.", "warning")
    return redirect(url_for("termino_clave_list"))

@app.route("/termino_clave/editar/<path:termino>", methods=["POST"])
@roles_required("Administrador", "Editor")
def termino_clave_update(termino):
    encoded = quote(termino, safe="")
    payload = {
        "termino": termino,
        "terminoIngles": request.form.get("termino_ingles", "").strip() or None
    }
    resp = api_call("PUT", f"termino_clave/{encoded}", payload)
    if resp and resp.status_code == 200:
        flash("Término clave actualizado exitosamente.", "success")
        return redirect(url_for("termino_clave_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/termino_clave/form.html", item=payload, is_new=False)

@app.route("/termino_clave/eliminar/<path:termino>", methods=["POST"])
@roles_required("Administrador")
def termino_clave_delete(termino):
    encoded = quote(termino, safe="")
    resp = api_call("DELETE", f"termino_clave/{encoded}")
    if resp and resp.status_code == 200:
        flash("Término clave eliminado lógicamente.", "success")
    else:
        flash("No se pudo eliminar el registro.", "danger")
    return redirect(url_for("termino_clave_list"))

# =============================================================================
# Módulo 5: Universidad
# =============================================================================
@app.route("/universidad")
@login_required
def universidad_list():
    resp = api_call("GET", "universidad")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/universidad/list.html", items=items)

@app.route("/universidad/nuevo")
@roles_required("Administrador", "Editor")
def universidad_new():
    return render_template("modulos/universidad/form.html", item=None)

@app.route("/universidad/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def universidad_create():
    payload = {
        "nombre": request.form.get("nombre", "").strip(),
        "tipo": request.form.get("tipo", "").strip(),
        "ciudad": request.form.get("ciudad", "").strip()
    }
    resp = api_call("POST", "universidad", payload)
    if resp and resp.status_code == 201:
        flash("Universidad creada exitosamente.", "success")
        return redirect(url_for("universidad_list"))
    error_msg = resp.json().get("mensaje", "Error al crear.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/universidad/form.html", item=payload)

@app.route("/universidad/editar/<int:id>")
@roles_required("Administrador", "Editor")
def universidad_edit(id):
    resp = api_call("GET", f"universidad/{id}")
    if resp and resp.status_code == 200:
        return render_template("modulos/universidad/form.html", item=resp.json())
    flash("Registro no encontrado o inactivo.", "warning")
    return redirect(url_for("universidad_list"))

@app.route("/universidad/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def universidad_update(id):
    payload = {
        "id": id,
        "nombre": request.form.get("nombre", "").strip(),
        "tipo": request.form.get("tipo", "").strip(),
        "ciudad": request.form.get("ciudad", "").strip()
    }
    resp = api_call("PUT", f"universidad/{id}", payload)
    if resp and resp.status_code == 200:
        flash("Universidad actualizada exitosamente.", "success")
        return redirect(url_for("universidad_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/universidad/form.html", item=payload)

@app.route("/universidad/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def universidad_delete(id):
    resp = api_call("DELETE", f"universidad/{id}")
    if resp and resp.status_code == 200:
        flash("Universidad eliminada lógicamente.", "success")
    else:
        flash("No se pudo eliminar el registro.", "danger")
    return redirect(url_for("universidad_list"))

# =============================================================================
# Módulo 6: Línea de Investigación
# =============================================================================
@app.route("/linea_investigacion")
@login_required
def linea_investigacion_list():
    resp = api_call("GET", "linea_investigacion")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/linea_investigacion/list.html", items=items)

@app.route("/linea_investigacion/nuevo")
@roles_required("Administrador", "Editor")
def linea_investigacion_new():
    return render_template("modulos/linea_investigacion/form.html", item=None)

@app.route("/linea_investigacion/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def linea_investigacion_create():
    payload = {
        "nombre": request.form.get("nombre", "").strip(),
        "descripcion": request.form.get("descripcion", "").strip() or None
    }
    resp = api_call("POST", "linea_investigacion", payload)
    if resp and resp.status_code == 201:
        flash("Línea de investigación creada exitosamente.", "success")
        return redirect(url_for("linea_investigacion_list"))
    error_msg = resp.json().get("mensaje", "Error al crear.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/linea_investigacion/form.html", item=payload)

@app.route("/linea_investigacion/editar/<int:id>")
@roles_required("Administrador", "Editor")
def linea_investigacion_edit(id):
    resp = api_call("GET", f"linea_investigacion/{id}")
    if resp and resp.status_code == 200:
        return render_template("modulos/linea_investigacion/form.html", item=resp.json())
    flash("Registro no encontrado o inactivo.", "warning")
    return redirect(url_for("linea_investigacion_list"))

@app.route("/linea_investigacion/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def linea_investigacion_update(id):
    payload = {
        "id": id,
        "nombre": request.form.get("nombre", "").strip(),
        "descripcion": request.form.get("descripcion", "").strip() or None
    }
    resp = api_call("PUT", f"linea_investigacion/{id}", payload)
    if resp and resp.status_code == 200:
        flash("Línea de investigación actualizada exitosamente.", "success")
        return redirect(url_for("linea_investigacion_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/linea_investigacion/form.html", item=payload)

@app.route("/linea_investigacion/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def linea_investigacion_delete(id):
    resp = api_call("DELETE", f"linea_investigacion/{id}")
    if resp and resp.status_code == 200:
        flash("Línea de investigación eliminada lógicamente.", "success")
    else:
        flash("No se pudo eliminar el registro.", "danger")
    return redirect(url_for("linea_investigacion_list"))

# =============================================================================
# Módulo 7: Docente (HU 3)
# =============================================================================
def _get_lineas_activas():
    resp = api_call("GET", "linea_investigacion")
    return resp.json() if resp and resp.status_code == 200 else []

@app.route("/docente")
@login_required
def docente_list():
    resp = api_call("GET", "docente")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/docente/list.html", items=items)

@app.route("/docente/nuevo")
@roles_required("Administrador", "Editor")
def docente_new():
    return render_template("modulos/docente/form.html", item=None, lineas=_get_lineas_activas(), is_new=True)

@app.route("/docente/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def docente_create():
    cedula_val = request.form.get("cedula", "").strip()
    linea_val = request.form.get("linea_investigacion_principal", "").strip()
    payload = {
        "cedula": int(cedula_val) if cedula_val.isdigit() else 0,
        "nombres": request.form.get("nombres", "").strip(),
        "apellidos": request.form.get("apellidos", "").strip(),
        "correo": request.form.get("correo", "").strip(),
        "enlaceCvlac": request.form.get("enlace_cvlac", "").strip() or None,
        "escalafon": request.form.get("escalafon", "").strip() or None,
        "categoriaMinciencias": request.form.get("categoria_minciencias", "").strip() or None,
        "lineaInvestigacionPrincipal": int(linea_val) if linea_val.isdigit() else None
    }
    resp = api_call("POST", "docente", payload)
    if resp and resp.status_code == 201:
        flash("Docente registrado exitosamente.", "success")
        return redirect(url_for("docente_list"))
    error_msg = resp.json().get("mensaje", "Error al crear docente.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/docente/form.html", item=payload, lineas=_get_lineas_activas(), is_new=True)

@app.route("/docente/editar/<int:cedula>")
@roles_required("Administrador", "Editor")
def docente_edit(cedula):
    resp = api_call("GET", f"docente/{cedula}")
    if resp and resp.status_code == 200:
        return render_template("modulos/docente/form.html", item=resp.json(), lineas=_get_lineas_activas(), is_new=False)
    flash("Docente no encontrado o inactivo.", "warning")
    return redirect(url_for("docente_list"))

@app.route("/docente/editar/<int:cedula>", methods=["POST"])
@roles_required("Administrador", "Editor")
def docente_update(cedula):
    linea_val = request.form.get("linea_investigacion_principal", "").strip()
    payload = {
        "cedula": cedula,
        "nombres": request.form.get("nombres", "").strip(),
        "apellidos": request.form.get("apellidos", "").strip(),
        "correo": request.form.get("correo", "").strip(),
        "enlaceCvlac": request.form.get("enlace_cvlac", "").strip() or None,
        "escalafon": request.form.get("escalafon", "").strip() or None,
        "categoriaMinciencias": request.form.get("categoria_minciencias", "").strip() or None,
        "lineaInvestigacionPrincipal": int(linea_val) if linea_val.isdigit() else None
    }
    resp = api_call("PUT", f"docente/{cedula}", payload)
    if resp and resp.status_code == 200:
        flash("Docente actualizado exitosamente.", "success")
        return redirect(url_for("docente_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/docente/form.html", item=payload, lineas=_get_lineas_activas(), is_new=False)

@app.route("/docente/eliminar/<int:cedula>", methods=["POST"])
@roles_required("Administrador")
def docente_delete(cedula):
    resp = api_call("DELETE", f"docente/{cedula}")
    if resp and resp.status_code == 200:
        flash("Docente desactivado exitosamente (borrado lógico).", "success")
    else:
        flash("No se pudo desactivar el docente.", "danger")
    return redirect(url_for("docente_list"))

# =============================================================================
# Módulo 8: Grupo de Investigación (HU 5)
# =============================================================================
def _get_universidades_activas():
    resp = api_call("GET", "universidad")
    return resp.json() if resp and resp.status_code == 200 else []

def _get_docentes_activos():
    resp = api_call("GET", "docente")
    return resp.json() if resp and resp.status_code == 200 else []

@app.route("/grupo_investigacion")
@login_required
def grupo_investigacion_list():
    resp = api_call("GET", "grupo_investigacion")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/grupo_investigacion/list.html", items=items)

@app.route("/grupo_investigacion/nuevo")
@roles_required("Administrador", "Editor")
def grupo_investigacion_new():
    return render_template(
        "modulos/grupo_investigacion/form.html",
        item=None,
        universidades=_get_universidades_activas(),
        docentes=_get_docentes_activos()
    )

@app.route("/grupo_investigacion/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def grupo_investigacion_create():
    u_id = request.form.get("universidad_id", "").strip()
    d_cedula = request.form.get("docente_lider_cedula", "").strip()
    payload = {
        "codigo": request.form.get("codigo", "").strip(),
        "nombre": request.form.get("nombre", "").strip(),
        "enlaceGruplac": request.form.get("enlace_gruplac", "").strip() or None,
        "categoriaMinciencias": request.form.get("categoria_minciencias", "").strip() or None,
        "universidadId": int(u_id) if u_id.isdigit() else 0,
        "docenteLiderCedula": int(d_cedula) if d_cedula.isdigit() else 0
    }
    resp = api_call("POST", "grupo_investigacion", payload)
    if resp and resp.status_code == 201:
        flash("Grupo de investigación registrado exitosamente.", "success")
        return redirect(url_for("grupo_investigacion_list"))
    error_msg = resp.json().get("mensaje", "Error al registrar grupo.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template(
        "modulos/grupo_investigacion/form.html",
        item=payload,
        universidades=_get_universidades_activas(),
        docentes=_get_docentes_activos()
    )

@app.route("/grupo_investigacion/editar/<int:id>")
@roles_required("Administrador", "Editor")
def grupo_investigacion_edit(id):
    resp = api_call("GET", f"grupo_investigacion/{id}")
    if resp and resp.status_code == 200:
        return render_template(
            "modulos/grupo_investigacion/form.html",
            item=resp.json(),
            universidades=_get_universidades_activas(),
            docentes=_get_docentes_activos()
        )
    flash("Grupo de investigación no encontrado o inactivo.", "warning")
    return redirect(url_for("grupo_investigacion_list"))

@app.route("/grupo_investigacion/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def grupo_investigacion_update(id):
    u_id = request.form.get("universidad_id", "").strip()
    d_cedula = request.form.get("docente_lider_cedula", "").strip()
    payload = {
        "id": id,
        "codigo": request.form.get("codigo", "").strip(),
        "nombre": request.form.get("nombre", "").strip(),
        "enlaceGruplac": request.form.get("enlace_gruplac", "").strip() or None,
        "categoriaMinciencias": request.form.get("categoria_minciencias", "").strip() or None,
        "universidadId": int(u_id) if u_id.isdigit() else 0,
        "docenteLiderCedula": int(d_cedula) if d_cedula.isdigit() else 0
    }
    resp = api_call("PUT", f"grupo_investigacion/{id}", payload)
    if resp and resp.status_code == 200:
        flash("Grupo de investigación actualizado exitosamente.", "success")
        return redirect(url_for("grupo_investigacion_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar grupo.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template(
        "modulos/grupo_investigacion/form.html",
        item=payload,
        universidades=_get_universidades_activas(),
        docentes=_get_docentes_activos()
    )

@app.route("/grupo_investigacion/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def grupo_investigacion_delete(id):
    resp = api_call("DELETE", f"grupo_investigacion/{id}")
    if resp and resp.status_code == 200:
        flash("Grupo de investigación desactivado exitosamente (borrado lógico).", "success")
    else:
        flash("No se pudo desactivar el grupo de investigación.", "danger")
    return redirect(url_for("grupo_investigacion_list"))

# =============================================================================
# Módulo 9: Semillero de Investigación (HU 6)
# =============================================================================
def _get_grupos_activos():
    resp = api_call("GET", "grupo_investigacion")
    return resp.json() if resp and resp.status_code == 200 else []

@app.route("/semillero")
@login_required
def semillero_list():
    resp = api_call("GET", "semillero")
    items = resp.json() if resp and resp.status_code == 200 else []
    if resp is None:
        flash("No fue posible conectar con la API REST.", "danger")
    return render_template("modulos/semillero/list.html", items=items)

@app.route("/semillero/nuevo")
@roles_required("Administrador", "Editor")
def semillero_new():
    return render_template("modulos/semillero/form.html", item=None, grupos=_get_grupos_activos())

@app.route("/semillero/crear", methods=["POST"])
@roles_required("Administrador", "Editor")
def semillero_create():
    g_id = request.form.get("grupo_investigacion_id", "").strip()
    payload = {
        "nombre": request.form.get("nombre", "").strip(),
        "descripcion": request.form.get("descripcion", "").strip() or None,
        "grupoInvestigacionId": int(g_id) if g_id.isdigit() else 0
    }
    resp = api_call("POST", "semillero", payload)
    if resp and resp.status_code == 201:
        flash("Semillero creado exitosamente.", "success")
        return redirect(url_for("semillero_list"))
    error_msg = resp.json().get("mensaje", "Error al crear semillero.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/semillero/form.html", item=payload, grupos=_get_grupos_activos())

@app.route("/semillero/editar/<int:id>")
@roles_required("Administrador", "Editor")
def semillero_edit(id):
    resp = api_call("GET", f"semillero/{id}")
    if resp and resp.status_code == 200:
        return render_template("modulos/semillero/form.html", item=resp.json(), grupos=_get_grupos_activos())
    flash("Semillero no encontrado o inactivo.", "warning")
    return redirect(url_for("semillero_list"))

@app.route("/semillero/editar/<int:id>", methods=["POST"])
@roles_required("Administrador", "Editor")
def semillero_update(id):
    g_id = request.form.get("grupo_investigacion_id", "").strip()
    payload = {
        "id": id,
        "nombre": request.form.get("nombre", "").strip(),
        "descripcion": request.form.get("descripcion", "").strip() or None,
        "grupoInvestigacionId": int(g_id) if g_id.isdigit() else 0
    }
    resp = api_call("PUT", f"semillero/{id}", payload)
    if resp and resp.status_code == 200:
        flash("Semillero actualizado exitosamente.", "success")
        return redirect(url_for("semillero_list"))
    error_msg = resp.json().get("mensaje", "Error al actualizar semillero.") if resp else "Error de conexión."
    flash(f"Error: {error_msg}", "danger")
    return render_template("modulos/semillero/form.html", item=payload, grupos=_get_grupos_activos())

@app.route("/semillero/eliminar/<int:id>", methods=["POST"])
@roles_required("Administrador")
def semillero_delete(id):
    resp = api_call("DELETE", f"semillero/{id}")
    if resp and resp.status_code == 200:
        flash("Semillero desactivado exitosamente (borrado lógico).", "success")
    else:
        flash("No se pudo desactivar el semillero.", "danger")
    return redirect(url_for("semillero_list"))

# =============================================================================
# Módulo 10: Relaciones y Articulaciones (Tablas Puente HU 5, 6, 8, 9)
# =============================================================================
@app.route("/relaciones")
@login_required
def relaciones_dashboard():
    docentes = _get_docentes_activos()
    grupos = _get_grupos_activos()
    semilleros = api_call("GET", "semillero").json() if api_call("GET", "semillero") else []
    lineas = _get_lineas_activas()
    areas = api_call("GET", "area_conocimiento").json() if api_call("GET", "area_conocimiento") else []
    ods = api_call("GET", "objetivo_desarrollo_sostenible").json() if api_call("GET", "objetivo_desarrollo_sostenible") else []
    aplicaciones = api_call("GET", "area_aplicacion").json() if api_call("GET", "area_aplicacion") else []

    dg = api_call("GET", "docente_grupo").json() if api_call("GET", "docente_grupo") else []
    ds = api_call("GET", "docente_semillero").json() if api_call("GET", "docente_semillero") else []
    gl = api_call("GET", "grupo_linea").json() if api_call("GET", "grupo_linea") else []
    sl = api_call("GET", "semillero_linea").json() if api_call("GET", "semillero_linea") else []
    la = api_call("GET", "linea_area").json() if api_call("GET", "linea_area") else []
    lo = api_call("GET", "linea_ods").json() if api_call("GET", "linea_ods") else []
    laa = api_call("GET", "linea_area_aplicacion").json() if api_call("GET", "linea_area_aplicacion") else []

    return render_template(
        "modulos/relaciones/index.html",
        docentes=docentes,
        grupos=grupos,
        semilleros=semilleros,
        lineas=lineas,
        areas=areas,
        ods=ods,
        aplicaciones=aplicaciones,
        rel_dg=dg,
        rel_ds=ds,
        rel_gl=gl,
        rel_sl=sl,
        rel_la=la,
        rel_lo=lo,
        rel_laa=laa
    )

@app.route("/relaciones/crear/<tipo>", methods=["POST"])
@roles_required("Administrador", "Editor")
def relaciones_create(tipo):
    path_map = {
        "docente_grupo": "docente_grupo",
        "docente_semillero": "docente_semillero",
        "grupo_linea": "grupo_linea",
        "semillero_linea": "semillero_linea",
        "linea_area": "linea_area",
        "linea_ods": "linea_ods",
        "linea_area_aplicacion": "linea_area_aplicacion"
    }
    if tipo not in path_map:
        flash("Tipo de relación no válido.", "danger")
        return redirect(url_for("relaciones_dashboard"))

    payload = {}
    if tipo == "docente_grupo":
        payload = {
            "docenteCedula": int(request.form.get("docente_cedula", 0)),
            "grupoId": int(request.form.get("grupo_id", 0)),
            "rol": request.form.get("rol", "").strip(),
            "fechaInicio": request.form.get("fecha_inicio", "") + "T00:00:00Z",
            "fechaFin": (request.form.get("fecha_fin", "") + "T00:00:00Z") if request.form.get("fecha_fin") else None
        }
    elif tipo == "docente_semillero":
        payload = {
            "docenteCedula": int(request.form.get("docente_cedula", 0)),
            "semilleroId": int(request.form.get("semillero_id", 0)),
            "rol": request.form.get("rol", "").strip(),
            "periodoAcademico": request.form.get("periodo_academico", "").strip()
        }
    elif tipo == "grupo_linea":
        payload = {
            "grupoId": int(request.form.get("grupo_id", 0)),
            "lineaId": int(request.form.get("linea_id", 0))
        }
    elif tipo == "semillero_linea":
        payload = {
            "semilleroId": int(request.form.get("semillero_id", 0)),
            "lineaId": int(request.form.get("linea_id", 0))
        }
    elif tipo == "linea_area":
        payload = {
            "lineaId": int(request.form.get("linea_id", 0)),
            "areaConocimientoId": int(request.form.get("area_conocimiento_id", 0))
        }
    elif tipo == "linea_ods":
        payload = {
            "lineaId": int(request.form.get("linea_id", 0)),
            "odsId": int(request.form.get("ods_id", 0))
        }
    elif tipo == "linea_area_aplicacion":
        payload = {
            "lineaId": int(request.form.get("linea_id", 0)),
            "areaAplicacionId": int(request.form.get("area_aplicacion_id", 0))
        }

    resp = api_call("POST", path_map[tipo], payload)
    if resp and (resp.status_code == 200 or resp.status_code == 201):
        flash("Relación vinculada exitosamente.", "success")
    else:
        error_msg = resp.json().get("mensaje", "Error al vincular relación.") if resp else "Error de conexión."
        flash(f"Error: {error_msg}", "danger")
    return redirect(url_for("relaciones_dashboard"))

@app.route("/relaciones/eliminar/<tipo>", methods=["POST"])
@roles_required("Administrador")
def relaciones_delete(tipo):
    path_map = {
        "docente_grupo": "docente_grupo",
        "docente_semillero": "docente_semillero",
        "grupo_linea": "grupo_linea",
        "semillero_linea": "semillero_linea",
        "linea_area": "linea_area",
        "linea_ods": "linea_ods",
        "linea_area_aplicacion": "linea_area_aplicacion"
    }
    if tipo not in path_map:
        flash("Tipo de relación no válido.", "danger")
        return redirect(url_for("relaciones_dashboard"))

    params = {}
    if tipo == "docente_grupo":
        params = {
            "docenteCedula": request.form.get("docente_cedula"),
            "grupoId": request.form.get("grupo_id")
        }
    elif tipo == "docente_semillero":
        params = {
            "docenteCedula": request.form.get("docente_cedula"),
            "semilleroId": request.form.get("semillero_id"),
            "periodoAcademico": request.form.get("periodo_academico")
        }
    elif tipo == "grupo_linea":
        params = {
            "grupoId": request.form.get("grupo_id"),
            "lineaId": request.form.get("linea_id")
        }
    elif tipo == "semillero_linea":
        params = {
            "semilleroId": request.form.get("semillero_id"),
            "lineaId": request.form.get("linea_id")
        }
    elif tipo == "linea_area":
        params = {
            "lineaId": request.form.get("linea_id"),
            "areaConocimientoId": request.form.get("area_conocimiento_id")
        }
    elif tipo == "linea_ods":
        params = {
            "lineaId": request.form.get("linea_id"),
            "odsId": request.form.get("ods_id")
        }
    elif tipo == "linea_area_aplicacion":
        params = {
            "lineaId": request.form.get("linea_id"),
            "areaAplicacionId": request.form.get("area_aplicacion_id")
        }

    resp = api_call("DELETE", path_map[tipo], params=params)
    if resp and resp.status_code == 200:
        flash("Relación desactivada exitosamente (borrado lógico).", "success")
    else:
        flash("No se pudo desactivar la relación.", "danger")
    return redirect(url_for("relaciones_dashboard"))

if __name__ == "__main__":
    port = int(os.getenv("PORT", 5000))
    app.run(host="0.0.0.0", port=port, debug=False)