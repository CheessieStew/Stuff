﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="deklarka2.aspx.cs" Inherits="z7_1.deklarka2" EnableViewState="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Deklarka" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    <div>

     				   <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td>Imię i nazwisko:</td>
                        <td><asp:Label ID="NameLabel" runat="server"/></td>
                        <td>Lista:</td>
                        <td><asp:Label ID="ListNoLabel" runat="server"/></td>
                    </tr>
                </table>
                <table border="0">
                    <tr>
                        <td>Zadanie:</td>
                        <asp:Repeater runat="server" ID="TaskNo">
                            <ItemTemplate>
                                <td> <%#Container.DataItem.ToString()%></td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <td>Data:</td>
                        <td><asp:Label ID="DateLabel" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Punkty:</td>
                        <asp:Repeater runat="server" ID="TaskPo">
                            <ItemTemplate>
                                <td> <%# PointsString(Container.DataItem)%></td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <td>suma:</td>
                        <td><asp:Label ID="SumLabel" runat="server" /></td>
                    </tr>
                </table>
        				            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="masterbutton" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <ajaxToolkit:UpdatePanelAnimationExtender runat="server" BehaviorID="UpdatePanel1_UpdatePanelAnimationExtender" TargetControlID="UpdatePanel1" ID="UpdatePanel1_UpdatePanelAnimationExtender">
            <Animations>
                <OnUpdated>
                    <Sequence>
                        <Parallel duration="1" Fps="30">
                            <FadeIn AnimationTarget="up_container" minimumOpacity="0"/>
                        </Parallel>
                    </Sequence>
                </OnUpdated>
            </Animations>

        </ajaxToolkit:UpdatePanelAnimationExtender>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
            <ProgressTemplate>
                Bzzzzt...
            </ProgressTemplate>
        </asp:UpdateProgress>
       <div>
            <table>
                <tr>
                    <td><asp:Label runat="server" Text="Imię:" /></td>
                    <td><asp:TextBox runat="server" ID="NameTB"/></td>
                </tr>
                <tr>
                    <td><asp:Label runat="server" Text="Nazwisko:" /></td>
                    <td><asp:TextBox runat="server" ID="SurnameTB" /></td>
                </tr>
                <tr>
                    <td><asp:Label runat="server" Text="Data:" /></td>
                    <td>
                        <asp:TextBox runat="server" ID="dateTB"/>
                        <ajaxToolkit:CalendarExtender ID="dateTB_CalendarExtender" runat="server" BehaviorID="dateTB_CalendarExtender" TargetControlID="dateTB" />
                    </td>
                </tr>
                <tr>
                    <td><asp:Label runat="server" Text="Numer zestawu:"/></td>
                    <td><asp:TextBox runat="server" type="number" ID="ListNoTB"/></td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>



                <asp:Repeater runat="server" ID="TasksRepeater" EnableViewState="True" ViewStateMode="Enabled">
                    <ItemTemplate >
                    <tr>
                        <td>Punkty za zad. <%#Container.DataItem.ToString()%>:</td>
                        <td>
                            <asp:TextBox runat="server" type="number" ID="TaskNTB"/>
                        </td>
                    </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <tr><td><asp:Button ID="masterbutton" runat="server" OnClick="masterbutton_Click" Text="master akt" /></td></tr>
            </table>
        </div>

    </div>
    </form>
</body>
</html>
