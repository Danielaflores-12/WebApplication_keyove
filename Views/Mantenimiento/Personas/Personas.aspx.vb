Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.WebApplication_Keyove.Model
Imports ModeloPersonas = WebApplication_keyove.WebApplication_Keyove.Model.Personas

Namespace WebApplication_Keyove.Views.Mantenimiento.Personas

    Partial Public Class Persona1
        Inherits Global.System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

        End Sub


        '==================================================
        ' LISTAR PERSONAS
        '==================================================

        <WebMethod()>
        <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
        Public Shared Function ListarPersonas() As List(Of ModeloPersonas)

            Dim lista As New List(Of ModeloPersonas)()

            Dim tabla As DataTable =
                New ModeloPersonas().ListaDatosShort()

            For Each fila As DataRow In tabla.Rows

                Dim obj As New ModeloPersonas()

                obj.iCodPersona =
                    Convert.ToInt32(fila("iCodPersona"))

                obj.cNombres =
                    Convert.ToString(fila("cNombres"))

                obj.cApellidos =
                    Convert.ToString(fila("cApellidos"))

                obj.cGenero =
                    Convert.ToString(fila("cGenero"))

                If IsDBNull(fila("cCorreo")) Then
                    obj.cCorreo = Nothing
                Else
                    obj.cCorreo =
                        Convert.ToString(fila("cCorreo"))
                End If

                If IsDBNull(fila("cTelefono")) Then
                    obj.cTelefono = Nothing
                Else
                    obj.cTelefono =
                        Convert.ToString(fila("cTelefono"))
                End If

                If IsDBNull(fila("dFechaNacimiento")) Then
                    obj.dFechaNacimiento = Nothing
                Else
                    obj.dFechaNacimiento =
                        Convert.ToDateTime(
                            fila("dFechaNacimiento")
                        )
                End If

                obj.dFechaRegistro =
                    Convert.ToDateTime(
                        fila("dFechaRegistro")
                    )

                lista.Add(obj)

            Next

            Return lista

        End Function


        '==================================================
        ' GUARDAR PERSONA
        '==================================================

        <WebMethod()>
        Public Shared Function GuardarPersona(persona As ModeloPersonas) As String

            Try

                persona.Insertar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' MODIFICAR PERSONA
        '==================================================

        <WebMethod()>
        Public Shared Function ModificarPersona(persona As ModeloPersonas) As String

            Try

                persona.Modificar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' ELIMINAR PERSONA
        '==================================================

        <WebMethod()>
        Public Shared Function EliminarPersona(idPersona As Integer) As String

            Try

                Dim objPersona As New ModeloPersonas()

                objPersona.iCodPersona = idPersona
                objPersona.Eliminar()

                Return "OK"

            Catch ex As Exception

                Return "ERROR: " & ex.Message

            End Try

        End Function


        '==================================================
        ' OBTENER PERSONA POR ID
        '==================================================

        <WebMethod()>
        Public Shared Function ObtenerPersona(idPersona As Integer) As ModeloPersonas

            Dim objPersona As New ModeloPersonas()

            objPersona.iCodPersona = idPersona
            objPersona.getRecord()

            Return objPersona

        End Function

    End Class

End Namespace