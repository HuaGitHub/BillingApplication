export interface Invoice{
    id: number;
    uploadedBy: string;
    uploadTimestamp: Date;
    fileSize: number;
    vendorName: string;
    invoiceDate: string;
    totalAmount: number;
    totalAmountDue: number;
    currency: string;
    taxAmount: number;
    processingStatus?: any;
}