ALTER   PROCEDURE [dbo].[sp_Rpt_AgedReceivable] 

@Date			varchar(25) ,
@Voucher_Date		INT =1 , 
@Due_Date		INT = 0

AS

Select tblGL_Balance.coa_detail_id AS Customer_Id, tblVendor.customer_name, ISNULL(tblGL_Balance.GL_Balance, 0) GL_Balance, ISNULL(tblTotal_Amount.Total_Amount, 0) Total_Amount, ISNULL(tblCurrent_Amount.Current_Amount, 0) Current_Amount, ISNULL(tbl30_60.[30-60], 0) [30-60], ISNULL(tbl60_90.[60-90], 0) [60-90], ISNULL(tbl90Plus.[90+], 0) [90+]  FROM
(SELECT     dbo.tblGlVoucherDetail.coa_detail_id, SUM(ISNULL(dbo.tblGlVoucherDetail.debit_amount, 0)) - SUM(ISNULL(dbo.tblGlVoucherDetail.credit_amount, 0)) AS GL_Balance
FROM         dbo.tblGlVoucher INNER JOIN
                      dbo.tblGlVoucherDetail ON dbo.tblGlVoucher.voucher_id = dbo.tblGlVoucherDetail.voucher_id
WHERE     

 dbo.tblGlVoucher.voucher_date <= CASE WHEN @Voucher_Date = 1  THEN CONVERT(datetime, @Date, 102) 
					ELSE	GETDATE()	
					END

AND
ISNULL(dbo.tblGlVoucher.due_date, '01-JAN-1900')  <= CASE WHEN @Due_Date = 1  THEN CONVERT(datetime, @Date, 102)
					ELSE	'2099-12-31'	
					END

AND (dbo.tblGlVoucher.voucher_no <> '000000') 
GROUP BY dbo.tblGlVoucherDetail.coa_detail_id)tblGL_Balance

RIGHT JOIN

(SELECT     tblVoucher.coa_detail_id, SUM(ISNULL(tblVoucher.debit, 0)) - SUM(ISNULL(tblCustomer.received, 0)) AS Total_Amount
FROM         (SELECT     tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id, tblGlVoucherdetail.coa_detail_id, SUM(tblGlVoucherdetail.debit_amount) debit
                       FROM          tblGlVoucherdetail INNER JOIN tblGlVoucher 
ON dbo.tblGlVoucher.voucher_id = dbo.tblGlVoucherDetail.voucher_id
WHERE      

dbo.tblGlVoucher.voucher_date <= CASE WHEN @Voucher_Date = 1  THEN CONVERT(datetime, @Date, 102)
					ELSE	GETDATE()	
					END

AND
dbo.tblGlVoucher.due_date <= CASE WHEN @Due_Date = 1  THEN  CONVERT(datetime, @Date, 102)
					ELSE	'2099-12-31'		
					END
AND tblGlVoucher.voucher_type_id=10
			GROUP BY tblGlVoucherdetail.coa_detail_id, tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id) tblVoucher LEFT JOIN
                          (SELECT     voucher_id, location_id, SUM(received_amount) received
                            FROM          tblGLCustomer_Receipts_Detail INNER JOIN
                      dbo.tblGLCustomer_Receipts ON dbo.tblGLCustomer_Receipts_Detail.CustomerReceipt_Id = dbo.tblGLCustomer_Receipts.customerReceipt_Id
WHERE     (dbo.tblGLCustomer_Receipts.receipt_Date <= CONVERT(datetime, @Date, 102))
                            GROUP BY voucher_id, location_id) tblCustomer ON tblVoucher.voucher_id = tblCustomer.voucher_id AND tblVoucher.location_id = tblCustomer.location_id
GROUP BY tblVoucher.coa_detail_id) tblTotal_Amount

ON
tblGL_Balance.coa_detail_id = tblTotal_Amount.coa_detail_id

LEFT JOIN 

(SELECT     tblVoucher.coa_detail_id, SUM(ISNULL(tblVoucher.debit, 0)) - SUM(ISNULL(tblCustomer.received, 0)) AS CURRENT_Amount
FROM         (SELECT     tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id, tblGlVoucherdetail.coa_detail_id, SUM(tblGlVoucherdetail.debit_amount) 
                                              debit
                       FROM          tblGlVoucherdetail INNER JOIN
                                              tblGlVoucher ON tblGlVoucher.voucher_id = tblGlVoucherdetail.voucher_id AND 
                                              tblGlVoucher.location_id = tblGlVoucherdetail.location_id
                       WHERE      

dbo.tblGlVoucher.voucher_date >= CASE WHEN @Voucher_Date = 1  THEN DATEADD(dd, -30, CONVERT(datetime, @Date, 102))
					ELSE	'01-JAN-1900'	
					END

AND
dbo.tblGlVoucher.voucher_date <= CASE WHEN @Voucher_Date = 1  THEN CONVERT(datetime, @Date, 102)
					ELSE	GETDATE()	
					END

AND 

dbo.tblGlVoucher.due_date >= CASE WHEN @Due_Date = 1  THEN DATEADD(dd, -30, CONVERT(datetime, @Date, 102))
					ELSE	'01-JAN-1900'	
					END

AND
dbo.tblGlVoucher.due_date <= CASE WHEN @Due_Date = 1  THEN CONVERT(datetime, @Date, 102)
					ELSE	'2099-12-31'		
					END

AND tblGlVoucher.voucher_type_id=10
                       GROUP BY tblGlVoucherdetail.coa_detail_id, tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id) tblVoucher LEFT JOIN
                          (SELECT     voucher_id, location_id, SUM(received_amount) received
                            FROM          tblGLCustomer_Receipts_Detail
                            GROUP BY voucher_id, location_id) tblCustomer ON tblVoucher.voucher_id = tblCustomer.voucher_id AND 
                      tblVoucher.location_id = tblCustomer.location_id
GROUP BY tblVoucher.coa_detail_id)tblCurrent_Amount

ON 
tblGL_Balance.coa_detail_id = tblCurrent_Amount.coa_detail_id

LEFT JOIN

(SELECT     tblVoucher.coa_detail_id, ISNULL(SUM(ISNULL(tblVoucher.debit, 0)) - SUM(ISNULL(tblCustomer.received, 0)),0) AS [30-60]
FROM         (SELECT     tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id, tblGlVoucherdetail.coa_detail_id, SUM(tblGlVoucherdetail.debit_amount) 
                                              debit
                       FROM          tblGlVoucherdetail INNER JOIN
                                              tblGlVoucher ON tblGlVoucher.voucher_id = tblGlVoucherdetail.voucher_id AND 
                                              tblGlVoucher.location_id = tblGlVoucherdetail.location_id
                       WHERE      

dbo.tblGlVoucher.voucher_date >= CASE WHEN @Voucher_Date = 1  THEN DATEADD(dd, -60, CONVERT(datetime, @Date, 102))
					ELSE	'01-JAN-1900'	
					END

AND
dbo.tblGlVoucher.voucher_date <= CASE WHEN @Voucher_Date = 1  THEN DATEADD(dd, -31, CONVERT(datetime, @Date, 102))
					ELSE	GETDATE()	
					END

AND 

dbo.tblGlVoucher.due_date >= CASE WHEN @Due_Date = 1  THEN DATEADD(dd, -60, CONVERT(datetime, @Date, 102))
					ELSE	'01-JAN-1900'	
					END

AND
dbo.tblGlVoucher.due_date <= CASE WHEN @Due_Date = 1  THEN DATEADD(dd, -31, CONVERT(datetime, @Date, 102))
					ELSE	'2099-12-31'		
					END
AND tblGlVoucher.voucher_type_id=10
                       GROUP BY tblGlVoucherdetail.coa_detail_id, tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id) tblVoucher LEFT JOIN
                          (SELECT     voucher_id, location_id, SUM(received_amount) received
                            FROM          tblGLCustomer_Receipts_Detail
                            GROUP BY voucher_id, location_id) tblCustomer ON tblVoucher.voucher_id = tblCustomer.voucher_id AND 
                      tblVoucher.location_id = tblCustomer.location_id
GROUP BY tblVoucher.coa_detail_id)tbl30_60

ON 
tblGL_Balance.coa_detail_id = tbl30_60.coa_detail_id

LEFT JOIN 
(SELECT     tblVoucher.coa_detail_id, SUM(ISNULL(tblVoucher.debit, 0)) - SUM(ISNULL(tblCustomer.received, 0)) AS [60-90]
FROM         (SELECT     tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id, tblGlVoucherdetail.coa_detail_id, SUM(tblGlVoucherdetail.debit_amount) 
                                              debit
                       FROM          tblGlVoucherdetail INNER JOIN
                                              tblGlVoucher ON tblGlVoucher.voucher_id = tblGlVoucherdetail.voucher_id AND 
                                              tblGlVoucher.location_id = tblGlVoucherdetail.location_id
                       WHERE      

dbo.tblGlVoucher.voucher_date >= CASE WHEN @Voucher_Date = 1  THEN DATEADD(dd, -90, CONVERT(datetime, @Date, 102))
					ELSE	'01-JAN-1900'	
					END

AND
dbo.tblGlVoucher.voucher_date <= CASE WHEN @Voucher_Date = 1  THEN DATEADD(dd, -61, CONVERT(datetime, @Date, 102))
					ELSE	GETDATE()	
					END

AND 

dbo.tblGlVoucher.due_date >= CASE WHEN @Due_Date = 1  THEN DATEADD(dd, -90, CONVERT(datetime, @Date, 102))
					ELSE	'01-JAN-1900'	
					END

AND
dbo.tblGlVoucher.due_date <= CASE WHEN @Due_Date = 1  THEN DATEADD(dd, -61, CONVERT(datetime, @Date, 102))
					ELSE	'2099-12-31'		
					END
AND tblGlVoucher.voucher_type_id=10
                       GROUP BY tblGlVoucherdetail.coa_detail_id, tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id) tblVoucher LEFT JOIN
                          (SELECT     voucher_id, location_id, SUM(received_amount) received
                            FROM          tblGLCustomer_Receipts_Detail
                            GROUP BY voucher_id, location_id) tblCustomer ON tblVoucher.voucher_id = tblCustomer.voucher_id AND 
                      tblVoucher.location_id = tblCustomer.location_id
GROUP BY tblVoucher.coa_detail_id)tbl60_90

ON

tblGL_Balance.coa_detail_id = tbl60_90.coa_detail_id

LEFT JOIN

(SELECT     tblVoucher.coa_detail_id, SUM(ISNULL(tblVoucher.debit, 0)) - SUM(ISNULL(tblCustomer.received, 0)) AS [90+]
FROM         (SELECT     tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id, tblGlVoucherdetail.coa_detail_id, SUM(tblGlVoucherdetail.debit_amount) 
                                              debit
                       FROM          tblGlVoucherdetail INNER JOIN
                                              tblGlVoucher ON tblGlVoucher.voucher_id = tblGlVoucherdetail.voucher_id AND 
                                              tblGlVoucher.location_id = tblGlVoucherdetail.location_id
                       WHERE      

dbo.tblGlVoucher.voucher_date < CASE WHEN @Voucher_Date = 1  THEN DATEADD(dd, -90, CONVERT(datetime, @Date, 102))
					ELSE	GETDATE()	
					END

AND
dbo.tblGlVoucher.due_date < CASE WHEN @Due_Date = 1  THEN DATEADD(dd, -90, CONVERT(datetime, @Date, 102))
					ELSE	'2099-12-31'		
					END
AND tblGlVoucher.voucher_type_id=10
                       GROUP BY tblGlVoucherdetail.coa_detail_id, tblGlVoucherdetail.voucher_id, tblGlVoucherdetail.location_id) tblVoucher LEFT JOIN
                          (SELECT     voucher_id, location_id, SUM(received_amount) received
                            FROM          tblGLCustomer_Receipts_Detail
                            GROUP BY voucher_id, location_id) tblCustomer ON tblVoucher.voucher_id = tblCustomer.voucher_id AND 
                      tblVoucher.location_id = tblCustomer.location_id
GROUP BY tblVoucher.coa_detail_id)tbl90Plus
ON

tblGL_Balance.coa_detail_id = tbl90Plus.coa_detail_id


INNER JOIN
(SELECT     dbo.tblGlCOAMainSubSubDetail.coa_detail_id AS Customer_Id, dbo.tblGlCOAMainSubSubDetail.detail_title AS customer_name
FROM         dbo.tblGlCOAMainSubSubDetail INNER JOIN
                      dbo.tblGlCOAMainSubSub ON dbo.tblGlCOAMainSubSubDetail.main_sub_sub_id = dbo.tblGlCOAMainSubSub.main_sub_sub_id
WHERE     ((dbo.tblGlCOAMainSubSub.account_type = 'Customer' OR dbo.tblGlCOAMainSubSub.account_type = 'Vendor') AND tblGlCoaMainSubSubDetail.end_date IS NULL)
)tblVendor

ON 

tblGL_Balance.coa_detail_id = tblVendor.customer_id



















