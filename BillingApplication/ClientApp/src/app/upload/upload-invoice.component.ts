import { Component, OnInit } from '@angular/core';
import { InvoiceService } from '../shared/services/invoice.service';
import { IUpload } from '../shared/interfaces/i-upload';
import { error } from 'protractor';

@Component({
  selector: 'app-upload-invoice',
  templateUrl: './upload-invoice.component.html',
  styleUrls: ['./upload-invoice.component.css']
})
export class UploadInvoiceComponent implements OnInit {
  uploadForm: IUpload = {
    file: '',
    email: ''
  };
  success: boolean;
  message: string;
  constructor(private invoiceService: InvoiceService) { }

  ngOnInit() {
  }

  //Upload invoice
  onUpload() {
    this.invoiceService.PostInvoice(this.uploadForm).subscribe(
      response => {
        if (response) {
          this.success = true;
          this.message = 'Upload successful!';
        }
        else {
          this.success = false;
          this.message = 'Upload failed';
        }
      },
      error => {
        this.success = false;
        this.message = 'Upload failed, please ensure file name is correct.';
      }
    )
  }
}
