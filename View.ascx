<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Vivaan.Modules.LogsAuto.View" %>

<div class="logs-auto-container">
    <h2>CSV Log Validator</h2>

    <asp:FileUpload ID="fuCsvUpload" runat="server" />
    <asp:Button ID="btnValidate" runat="server" Text="Validate CSV" OnClick="btnValidate_Click" CssClass="btn btn-primary" />

    <br /><br />

    <asp:Label ID="lblResult" runat="server" Font-Bold="true" />

    <br /><br />

    <asp:GridView ID="gvValidationResults" runat="server"
                  AutoGenerateColumns="false"
                  GridLines="Both"
                  ShowHeader="true"
                  BorderColor="#ccc"
                  BorderStyle="Solid"
                  BorderWidth="1px"
                  CellPadding="5"
                  CssClass="validation-grid">
        <Columns>
            <asp:BoundField DataField="Row" HeaderText="Row Number" />
            <asp:BoundField DataField="FailedValidations" HeaderText="Validation Errors" />
        </Columns>
    </asp:GridView>
</div>

<style>
    .logs-auto-container {
        padding: 20px;
        background-color: #f9f9f9;
        border: 1px solid #ddd;
        border-radius: 6px;
        max-width: 1600px;
    }

    /* Remove Bootstrap collapse behavior and apply visible borders */
    .validation-grid {
        border-collapse: collapse;
        width: 100%;
    }

    .validation-grid th,
    .validation-grid td {
        border: 1px solid #ccc;
        padding: 8px;
        text-align: left;
    }

    .validation-grid th {
        background-color: #f0f0f0;
        font-weight: bold;
    }
</style>
