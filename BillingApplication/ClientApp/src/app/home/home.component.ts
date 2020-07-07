import { Component, OnInit } from '@angular/core';
import { Invoice } from '../shared/interfaces/i-Invoice';
import { InvoiceService } from '../shared/services/invoice.service';
import { IStats } from '../shared/interfaces/i-stats';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  id: number;
  invoice: Invoice;
  errorMessage: string;
  errorMessage2: string;
  statsList: IStats[];

  constructor(private invoiceService: InvoiceService) { }

  ngOnInit() {
    this.errorMessage2 = '';

    //Get Statistics
    this.invoiceService.GetStats().subscribe(
      response => (this.statsList = response),
      error => {
        if (error.status == 404) {
          this.errorMessage2 = 'Statistics not found';
        }
        else {
          this.errorMessage2 = error.message;
        }
      }
    )
  }
  //Get invoice
  onSearch() {
    this.errorMessage = '';
    this.invoice = null;

    if (this.id) {
      this.invoiceService.GetInvoice(this.id).subscribe(
        response => { this.invoice = response; },
        error => {
          if (error.status == 404) {
            this.errorMessage = 'Invoice not found';
          }
          else {
            this.errorMessage = error.message;
          }
        }
      );
    }
  }
}
