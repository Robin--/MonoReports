Introduction to Monoreports
===========================

QuickStart
----------
If you can't wait to see something, here is quickstart to make 'hello world' report

1. run Monoreports designer

2. press F5

5. go back to design tab

6. drag *name* field and drop on details section

7. press F5

8. press export to pdf icon

Run report from code
--------------------
If you don't like/need designer, there is a project demonstrating how to create and run report from code in ****doc/example/MrptInvoiceExample****.

Basic Concepts
==============

What is Monoreports ?
---------------------
Monoreports is a reporting tool used to design and generate reports from object datasources. Datasource can anything from database data or file to dynamically generated data.

Monoreports consists of two main parts:

***report designer***  gtk-sharp application to design reports
***report engine***  engine is responsible for generating report from report designed in designer and data pushed to datasource

Report
------
Report is a template for result report. Every report has serveral sections. In sections you place controls.
Control can be bound to a datasource, parameter or expression.

Sections
--------

Section is empty space on the report. Sections differ by location and [...]

1. ***Report Header*** the first section printed at report top. It's printed once per report.
2. ***Page Header*** this section is printed at top of every page except first page (where report header is first)
3. ***Details*** this section is printed as many times as many rows there is in the data source
4. ***Page Footer*** printed at bottom of the page
5. ***Report Footer*** printed at the end of report

Controls
--------
There are three basic types of controls:

1. Textblock - represents text on report, can be bound to datafield, has background, border and font related properties
2. Line  - can be vertical horizontal (to make things easier in a designer)
3. Image - at the moment only static images are supported

and one more complex:

***Subreport control*** (currently not supported)



Datasource and parameters
==============
Every report have to be filled with data durning processing. There is field abstract to connnect visual controls on designed template with 
data comming from outside of report.
There are a three types of fields in Monoreports:
  *** parameter fields - used as report parameter
  *** data fields - used as field in datasource
  *** expression fields - used to display combination of data and parameter fields

                  e.g  To make your invoice number show in TextBox control, you have to define InvoiceNumber parameter field
                  then in textblock FieldName property enter this field name

Every datasource contains number of data fields (table  is a good parallel of what datasource is and DataField could be considered as Column).

Currently Monoreports has two build in datasource types:
- C# code based datasource - where ObjectDataSource<> is used
- JSON datasource - where JsonDatasource is used

C# code based datasource
-----------------
ObjectDataSource takes as contructor parameter IEnumerable<T>. 
Adding datafield to datasource is taking field name and lambda expression pointing to property of T

example:

			ObjectDataSource<InvoicePosition> objectDataSource = new ObjectDataSource<InvoicePosition>(invoice.Positions);
			objectDataSource.AddField ("Index",x=>x.Index);
			objectDataSource.AddField ("Description",x=>x.Description);
			objectDataSource.AddField ("Quantity",x=>x.Quantity);
			objectDataSource.AddField ("PricePerUnitGross",x=>x.PricePerUnitGross);					
			report.DataSource = objectDataSource;	

TODO add description

JSON based datasource
--------------------
Consider following JSON

              {
              "InvoiceNumber":"1/09/2010",
              "CreationDate":"/Date(1283292000000+0200)/",
              "SellDate":"/Date(1283292000000+0200)/",
              "CompanyName1":"   service solutions",
              "CompanyNIP":"PL773-212-38-22",
              "CustomerName1":"customer name",
              "CustomerNIP":"DE123423424234",
              "Positions":[
               {
               "Index":1,
               "Description":"my service description",
               "Unit":null,
               "Quantity":1,
               "PricePerUnitNet":2000,
               "ValueGross":2000,
               "TaxRate":0
               },
               {
               "Index":2,
               "Description":"my service description 2  my service description 2",
               "Unit":null,
               "Quantity":2,
               "PricePerUnitNet":344,
               "ValueGross":344,
               "TaxRate":0
               },
               ],
              "BankName":"XYZ Bank",
              "BankNumber":"IBAN PL952490000500004600461000",
              "TotalTax":0,
              "TotalGross":2344
               }


If JSON datasource is used every property is taken as report parameter and first array is taken as datasource, so in the above example
InvoiceNumber, Creation Date etc will be parameters, whereas Positions is the datasource. 

Main advantage of using JSON is that datas fields and parameter fields are autodicovered in designer.






