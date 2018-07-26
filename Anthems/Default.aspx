<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Anthems._Default" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Anthems - CDES</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="sm1" runat="server" />
        <div>
            Upload Audio (mp3):
            <asp:FileUpload ID="upload" runat="server" />
            <asp:Button ID="submitButton" runat="server" Text="Submit" OnClick="submitButton_Click" />
        </div>
        <div>
            <asp:UpdatePanel ID="up1" runat="server">
                <ContentTemplate>
                    <asp:ListView ID="AnthemDisplayControl" runat="server">
                        <LayoutTemplate>
                            <audio id='itemPlaceholder' src='#' runat="server"></audio>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <audio src='<%# Eval("Url") %>' controls runat="server"></audio>
                            <asp:Literal ID="audioFile" Text='<%# Eval("Title") %>' runat="server" />
                        </ItemTemplate>
                    </asp:ListView>
                    <asp:Timer ID="timer1" runat="server" Interval="30000" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
