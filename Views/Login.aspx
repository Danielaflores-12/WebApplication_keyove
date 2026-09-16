<%@ Page Language="vb" AutoEventWireup="false"
    CodeBehind="Login.aspx.vb"
    Inherits="WebApplication_keyove.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>KEYOVE | Acceso al sistema</title>

    <style>
        :root {
            --azul-oscuro: #081f33;
            --azul-medio: #0b4058;
            --turquesa: #0796a8;
            --turquesa-hover: #067b8a;
            --texto: #172033;
            --texto-suave: #64748b;
            --borde: #d8e0e7;
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
            font-family: "Segoe UI", Arial, sans-serif;
        }

        body {
            min-height: 100vh;
            background:
                radial-gradient(circle at 15% 15%, rgba(7, 150, 168, 0.18), transparent 28%),
                linear-gradient(135deg, #061725, #0a3044 60%, #0b5e6b);
        }

        .pagina {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px;
        }

        .tarjeta-login {
            width: 1040px;
            min-height: 610px;
            display: grid;
            grid-template-columns: 46% 54%;
            overflow: hidden;
            background: white;
            border: 1px solid rgba(255,255,255,0.20);
            border-radius: 18px;
            box-shadow: 0 28px 70px rgba(0, 0, 0, 0.35);
        }

        .panel-empresa {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 50px;
            text-align: center;
            color: white;
            background: linear-gradient(160deg, var(--azul-oscuro), var(--azul-medio));
        }

        .marca-keyove {
            width: 100%;
        }

        .caja-logo {
            width: 100%;
            max-width: 410px;
            padding: 18px;
            margin: 0 auto 30px;
            background: white;
            border-radius: 14px;
            box-shadow: 0 12px 30px rgba(0, 0, 0, 0.20);
        }

        .logo-keyove {
            display: block;
            width: 100%;
            height: auto;
        }

        .frase-marca {
            max-width: 340px;
            margin: auto;
            color: #cbe7ee;
            font-size: 18px;
            font-weight: 500;
            line-height: 1.6;
        }

        .panel-acceso {
            display: flex;
            flex-direction: column;
            justify-content: center;
            padding: 65px 70px;
            background: white;
        }

        .encabezado {
            padding-bottom: 25px;
            margin-bottom: 30px;
            border-bottom: 1px solid #e5eaf0;
        }

        .encabezado span {
            display: block;
            margin-bottom: 9px;
            color: var(--turquesa);
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 1.5px;
        }

        .encabezado h2 {
            margin-bottom: 8px;
            color: var(--texto);
            font-size: 30px;
        }

        .encabezado p {
            color: var(--texto-suave);
            font-size: 15px;
        }

        .grupo {
            margin-bottom: 22px;
        }

        .grupo label {
            display: block;
            margin-bottom: 8px;
            color: #263449;
            font-size: 14px;
            font-weight: 650;
        }

        .entrada {
            width: 100%;
            height: 52px;
            padding: 0 16px;
            border: 1px solid var(--borde);
            border-radius: 7px;
            outline: none;
            background: #f9fbfc;
            color: var(--texto);
            font-size: 15px;
        }

        .entrada:focus {
            border-color: var(--turquesa);
            background: white;
            box-shadow: 0 0 0 3px rgba(7, 150, 168, 0.12);
        }

        .campo-contrasena {
            position: relative;
        }

        .campo-contrasena .entrada {
            padding-right: 50px;
        }

        .btn-ojo {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            width: 32px;
            height: 32px;
            display: flex;
            align-items: center;
            justify-content: center;
            border: none;
            background: transparent;
            color: #64748b;
            cursor: pointer;
        }

        .btn-ojo:hover {
            color: var(--turquesa);
        }

        .icono-ojo {
            width: 20px;
            height: 20px;
            fill: none;
            stroke: currentColor;
            stroke-width: 1.8;
            stroke-linecap: round;
            stroke-linejoin: round;
        }

        .btn-ojo.esta-visible .linea-ojo {
            display: none;
        }

        .boton {
            width: 100%;
            height: 52px;
            margin-top: 8px;
            border: none;
            border-radius: 7px;
            background: var(--turquesa);
            color: white;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
        }

        .boton:hover {
            background: var(--turquesa-hover);
        }

        .mensaje {
            display: block;
            min-height: 22px;
            margin-top: 16px;
            text-align: center;
            color: #c62828;
            font-size: 14px;
        }

        .seguridad {
            margin-top: 25px;
            padding-top: 20px;
            border-top: 1px solid #e5eaf0;
            color: #8491a3;
            text-align: center;
            font-size: 12px;
        }

        @media (max-width: 820px) {
            .tarjeta-login {
                width: 480px;
                grid-template-columns: 1fr;
            }

            .panel-empresa {
                padding: 35px;
            }

            .caja-logo {
                max-width: 330px;
                margin-bottom: 18px;
            }

            .frase-marca {
                font-size: 15px;
            }

            .panel-acceso {
                padding: 42px 35px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <main class="pagina">
            <section class="tarjeta-login">

                <aside class="panel-empresa">
                    <div class="marca-keyove">
                        <div class="caja-logo">
                            <img src="<%= ResolveUrl("~/Assets/logo-keyove.png") %>"
                                alt="KEYOVE Artículos de Computación"
                                class="logo-keyove" />
                        </div>

                        <p class="frase-marca">
                            Tecnología confiable para una gestión más inteligente.
                        </p>
                    </div>
                </aside>

                <section class="panel-acceso">
                    <div class="encabezado">
                        <span>PORTAL INTERNO</span>
                        <h2>Acceso al sistema</h2>
                        <p>Ingrese sus credenciales .</p>
                    </div>

                    <div class="grupo">
                        <label for="txtUsuario">Nombre de usuario</label>

                        <asp:TextBox
                            ID="txtUsuario"
                            runat="server"
                            CssClass="entrada"
                            placeholder="Ingrese su usuario"
                            autocomplete="username">
                        </asp:TextBox>
                    </div>

                    <div class="grupo">
                        <label for="txtContrasena">Contraseña</label>

                        <div class="campo-contrasena">
                            <asp:TextBox
                                ID="txtContrasena"
                                runat="server"
                                CssClass="entrada"
                                TextMode="Password"
                                placeholder="Ingrese su contraseña"
                                autocomplete="current-password">
                            </asp:TextBox>

                            <button type="button"
                                class="btn-ojo"
                                onclick="mostrarContrasena()"
                                title="Mostrar contraseña"
                                aria-label="Mostrar contraseña">

                                <svg class="icono-ojo" viewBox="0 0 24 24" aria-hidden="true">
                                    <path d="M2.5 12s3.4-5.5 9.5-5.5S21.5 12 21.5 12s-3.4 5.5-9.5 5.5S2.5 12 2.5 12Z"></path>
                                    <circle cx="12" cy="12" r="2.5"></circle>
                                    <path class="linea-ojo" d="M4 4 20 20"></path>
                                </svg>
                            </button>
                        </div>
                    </div>

                    <asp:Button
    ID="btnIngresar"
    runat="server"
    Text="INGRESAR"
    CssClass="boton"
    OnClick="btnIngresar_Click" />

                    <asp:Label
                        ID="lblMensaje"
                        runat="server"
                        CssClass="mensaje">
                    </asp:Label>

                    <p class="seguridad">
                        Acceso exclusivo para personal autorizado de KEYOVE
                    </p>
                </section>

            </section>
        </main>

        <script>
            function mostrarContrasena() {
                var campo = document.getElementById('txtContrasena');
                var boton = document.querySelector('.btn-ojo');

                if (!(campo instanceof HTMLInputElement) ||
                    !(boton instanceof HTMLButtonElement)) {
                    return;
                }

                var mostrar = campo.type === 'password';

                if (mostrar) {
                    campo.type = 'text';
                    boton.classList.add('esta-visible');
                    boton.title = 'Ocultar contraseña';
                } else {
                    campo.type = 'password';
                    boton.classList.remove('esta-visible');
                    boton.title = 'Mostrar contraseña';
                }
            }
        </script>
    </form>
</body>
</html>