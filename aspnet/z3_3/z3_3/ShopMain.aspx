<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopMain.aspx.cs" Inherits="z3_3.ShopMain" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ListView ID="ListView1" runat="server" DataSourceID="ObjectDataSource1" DataKeyNames="ID"
            ItemPlaceholderID="ListPlaceholder" OnItemCommand="ListView1_ItemCommand">
            <LayoutTemplate>
                <table>
                    <tr>
                        <td>
                            <asp:LinkButton ID="NameTH" runat="server" Text="Nazwa" CommandName="Sort"
                                CommandArgument="NAME" />
                        </td>
                        <td>
                            <asp:LinkButton ID="PriceTH" runat="server" Text="Cena" CommandName="Sort"
                                CommandArgument="PRICE" />
                        </td>
                        <td>
                            Opis
                        </td>
                        <td>
                            Obrazek
                        </td>
                    </tr>
                    <tr>
                        <asp:PlaceHolder ID="ListPlaceholder" runat="server"></asp:PlaceHolder>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:DataPager ID="DataPager" runat="server" PagedControlID="ListView1" PageSize="10">
                                <Fields>
                                    <asp:NumericPagerField ButtonCount="8" ButtonType="Link" />
                                </Fields>
                            </asp:DataPager>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:LinkButton ID="btDodaj" runat="server" CommandName="ShowInsertTemplate" Text="Dodaj nowy" />
                        </td>
                    </tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# ((z3_3.ShopItem)Container.DataItem).name %>
                    </td>
                    <td>
                        <%# ((z3_3.ShopItem)Container.DataItem).price.ToString("0.00")+"$" %>
                    </td>
                    <td>
                        <%# ((z3_3.ShopItem)Container.DataItem).description %>
                    </td>
                    <td>
                        <a href="<%# ((z3_3.ShopItem)Container.DataItem).picLink%>">Picture</a>
                    </td>
                    <td>
                        <asp:LinkButton ID="EditButton" CommandName="Edit" CommandArgument='<%# ((z3_3.ShopItem)Container.DataItem).ID %>'
                            Text="Edytuj" runat="server" />
                        <asp:LinkButton ID="DeleteButton" CommandName="Delete" CommandArgument='<%# ((z3_3.ShopItem)Container.DataItem).ID %>'
                            Text="Usuń" runat="server" />
                    </td>
                    <td>
                        <asp:LinkButton ID="DownloadXmlButton" CommandName="DownloadXml" CommandArgument='<%# ((z3_3.ShopItem)Container.DataItem).ID %>'
                            Text="Pobierz XML" runat="server" />
                    </td>
                </tr>
            </ItemTemplate>
            <InsertItemTemplate>
                <tr>
                    <td colspan="5">
                        <table width="60%">
                            <tr>
                                <td>
                                    Nazwa
                                </td>
                                <td width="100%">
                                    <asp:TextBox runat="server" ID="NameInsert" Width="60%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Cena
                                </td>
                                <td width="100%">
                                    <asp:TextBox runat="server" ID="PriceInsert" Width="60%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Opis
                                </td>
                                <td width="100%">
                                    <asp:TextBox runat="server" ID="DescriptionInsert" Width="60%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Obrazek
                                </td>
                                <td width="100%">
                                    <asp:TextBox runat="server" ID="PictureInsert" Width="60%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:LinkButton ID="Akceptuj" runat="server" Text="Akceptuj" CommandName="Insert" />
                                    <asp:LinkButton ID="Anuluj" runat="server" Text="Anuluj" CommandName="HideInsertTemplate" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </InsertItemTemplate>
            <EditItemTemplate>
                <tr>
                    <td colspan="5">
                        <table>
                            <tr>
                                <td>
                                    Nazwa
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="NameEdit" Width="60%" Text='<%# ((z3_3.ShopItem)Container.DataItem).name %>'></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Cena
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="PriceEdit" Width="60%" Text='<%# ((z3_3.ShopItem)Container.DataItem).price %>'></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Opis
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="DescriptionEdit" Width="60%" Text='<%# ((z3_3.ShopItem)Container.DataItem).description %>'></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Obrazek
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="PictureEdit" Width="60%" Text='<%# ((z3_3.ShopItem)Container.DataItem).picLink %>'></asp:TextBox>
                                </td>
                            </tr>                            <tr>
                                <td colspan="2">
                                    <asp:LinkButton ID="Akceptuj" runat="server" Text="Akceptuj" CommandName="Update" />
                                    <asp:LinkButton ID="Anuluj" runat="server" Text="Anuluj" CommandName="Cancel" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </EditItemTemplate>
        </asp:ListView>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" EnablePaging="True" 
            MaximumRowsParameterName="RowCount" SelectCountMethod="SelectCount" SelectMethod="Retrieve"
            SortParameterName="OrderBy" StartRowIndexParameterName="StartRow" TypeName="z3_3.ShopItem_DataProvider"

            DeleteMethod="Delete"
            OnDeleting="ObjectDataSource1_Deleting" 

            UpdateMethod="Update" 
            OnUpdating="ObjectDataSource1_Updating"
            OnUpdated="ObjectDataSource1_Updated" 

            InsertMethod="Insert"
            OnInserting="ObjectDataSource1_Inserting"
            OnInserted="ObjectDataSource1_Inserted"></asp:ObjectDataSource>
        <br />
        ListView w postaci listy wypunktowanej<br />
        ListView w postaci siatki szczegółów jednego elementu<br />
    </div>
    </form>
</body>
</html>
