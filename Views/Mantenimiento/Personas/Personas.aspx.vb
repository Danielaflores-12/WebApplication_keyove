Imports System.Web.Services
Imports System.Web.Script.Services
Imports WebApplication_keyove.Model

Public Class Persona1
    Inherits Global.System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As Global.System.EventArgs) Handles Me.Load

    End Sub


    '==================================================
    ' LISTAR PERSONAS
    '==================================================

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function ListarPersonas() As Object

        Dim objPersona As New Personas()

        Return objPersona.ListaDatosShort()

    End Function


    '==================================================
    ' GUARDAR PERSONA
    '==================================================

    <WebMethod()>
    Public Shared Function GuardarPersona(persona As Personas) As String

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
    Public Shared Function ModificarPersona(persona As Personas) As String

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

            Dim objPersona As New Personas()

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
    Public Shared Function ObtenerPersona(idPersona As Integer) As Personas

        Dim objPersona As New Personas()

        objPersona.iCodPersona = idPersona
        objPersona.getRecord()

        Return objPersona

    End Function

End Class