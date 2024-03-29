

--exec Sp_SalesTaxInvoice 24098,6
Create PROCEDURE [dbo].[Sp_SalesTaxInvoice]
	@voucher_id  int,
	@Location_id int
AS
BEGIN
--Get the mapped sales tax account_id
Declare @Sales_Tax_Acc int
set @sales_Tax_Acc=(select config_value from tblGlConfiguration where config_name='gl_ac_Services_Tax' )

--Get the sales Tax amount
Declare @Sales_Tax_Amt numeric(10,4)
set @Sales_Tax_Amt=(select credit_amount from tblGLVoucherDetail where voucher_id=@voucher_id and Location_id=@Location_id and coa_detail_id=@sales_Tax_Acc)

--Get Customer Discount
Declare @CustomerAddress nvarchar(max)
set @CustomerAddress =(SELECT     top(1) tblGLCustomerInforamtion.Address
FROM         tblGlCOAMainSubSubDetail INNER JOIN
                      tblGlVoucherDetail ON tblGlCOAMainSubSubDetail.coa_detail_id = tblGlVoucherDetail.coa_detail_id INNER JOIN
                      tblGLCustomerInforamtion ON tblGlCOAMainSubSubDetail.coa_detail_id = tblGLCustomerInforamtion.Account_id
WHERE     (tblGlVoucherDetail.voucher_id = @voucher_id) AND (tblGlVoucherDetail.location_id = @Location_id))

--Get the voucher Detail
SELECT     SUM(tblGlVoucherDetail.debit_amount) AS debit_amount, @CustomerAddress AS Address, @Sales_Tax_Amt AS 'sales_Tax_Amt'
FROM         tblGlCOAMainSubSubDetail INNER JOIN
                      tblGlVoucherDetail ON tblGlCOAMainSubSubDetail.coa_detail_id = tblGlVoucherDetail.coa_detail_id LEFT OUTER JOIN
                      tblGLCustomerInforamtion ON tblGlCOAMainSubSubDetail.coa_detail_id = tblGLCustomerInforamtion.Account_id
where tblGlVoucherDetail.voucher_id=@voucher_id and tblGlVoucherDetail.Location_id=@Location_id
END


