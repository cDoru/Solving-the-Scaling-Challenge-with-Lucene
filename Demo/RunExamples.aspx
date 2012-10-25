<%@ Page Language="C#" 
         AutoEventWireup="true" 
         CodeBehind="RunExamples.aspx.cs" 
         Inherits="Demo.RunExamples" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <asp:Button runat="server" ID="AddItems" Text="Add Items" OnClick="AddItems_OnClick"/>
        <asp:Button runat="server" ID="RunTests" Text="Run Tests" OnClick="RunTests_OnClick"/>
        <asp:Button runat="server" ID="RunAutomatedTests" Text="Run Automated Tests" OnClick="RunAutomatedTests_OnClick"/>
        <asp:Timer runat="server" ID="AddItemsTimers" OnTick="AddItems_Tick" Enabled="False"></asp:Timer>
        <asp:Timer runat="server" ID="RunExamplesTimer" OnTick="RunTests_Tick" Enabled="False"></asp:Timer>
        
        <p>Add Items Timer:</p>
        <p><asp:TextBox runat="server" ID="addItemsTimeInterval" Width="800px"></asp:TextBox></p>                
        <p>Run Test Items Timer:</p>
        <p><asp:TextBox runat="server" ID="runTestsTimeInterval" Width="800px"></asp:TextBox></p>        
        
        <p>
        Items:
        </p>
        <p><asp:TextBox runat="server" ID="numberOfItems" Width="800px"></asp:TextBox></p>
        
        <p>Results:</p>
        <p><asp:TextBox runat="server" ID="results" TextMode="MultiLine" Width="800px" Height="600px"></asp:TextBox></p>
        
        
        <asp:Literal runat="server" ID="resultsLiteral"></asp:Literal>
    </div>
    </form>
</body>
</html>
