Imports Microsoft.EnterpriseManagement.HealthService
Imports System.Xml

<MonitoringDataType>
Public Class JSONEncodedDataItem
    Inherits DataItemBase
    Private m_encodedItem As String
    ''' <summary>
    ''' JSON Encoded Data to be Persisted in the DataItem.
    ''' </summary>
    Private Const encodedItemTag As String = "JSON"


    ''' <summary>
    ''' Constructor to create a new instance of this data item.
    ''' </summary>
    ''' <param name="item">The value used to fill the output dataitem JSON Encoded</param>
    Public Sub New(item As String)
        If item Is Nothing Then
            Throw New ArgumentNullException("item")
        End If

        Me.m_encodedItem = item
    End Sub

    ''' <summary>
    ''' Constructor for creating this data item from XML.  
    ''' </summary>
    ''' <param name="reader">XmlReader containing the XML version of this data item.</param>
    Public Sub New(reader As XmlReader)
        MyBase.New(reader)
        'Read in relevant portion of the Data
        Try
            Me.m_encodedItem = reader.ReadElementString(encodedItemTag)
        Catch xe As XmlException
            Throw New MalformedDataItemException("This should be localized text about invalid XML in production.", xe)
        End Try

        Debug.Assert(Me.m_encodedItem IsNot Nothing)
    End Sub

    ''' <summary>
    ''' This is the name of the data item as specified in the MP.
    ''' "P2P.DataItems.JSONEncodedData" in this case
    ''' </summary>
    Public Overrides ReadOnly Property DataItemTypeName() As String
        Get
            Return "P2P.DataItems.JSONEncodedData"
        End Get
    End Property

    ''' <summary>
    ''' Data to be persisted and also posted on output stream.
    ''' </summary>
    Public ReadOnly Property EncodedItem() As String
        Get
            Return Me.m_encodedItem
        End Get
    End Property

    ''' <summary>
    ''' This writes out the portion of the XML that is specific to this
    ''' derived data item.
    ''' </summary>
    ''' <param name="writer">XML stream to write to.</param>
    Protected Overrides Sub GenerateItemXml(writer As XmlWriter)
        ' We just need to write out our portion of the XML document.  The
        ' base class has handled writing out its portion before it called
        ' into this method.
        writer.WriteElementString(JSONEncodedDataItem.encodedItemTag, Me.m_encodedItem)

    End Sub

End Class
