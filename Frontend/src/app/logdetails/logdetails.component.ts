import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LogService } from '../Services/log.service';
import { CommonModule } from '@angular/common';
import { ResponsAPI } from '../SharedClass/respons-api';

@Component({
  selector: 'app-logdetails',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './logdetails.component.html',
  styleUrl: './logdetails.component.css',
})
export class LogdetailsComponent implements OnInit {
  logId: number | null = null;
  logDetails: any = null;
  isLoading = true;
  errorMessage: string | null = null;
  serviceName: string | null = null;
  constructor(private route: ActivatedRoute, private _logService: LogService) {}
  ngOnInit(): void {
    this.logId = Number(this.route.snapshot.paramMap.get('id'));
    this.serviceName = this.route.snapshot.paramMap.get('serviceName');
    console.log(this.serviceName);
    if (this.logId != null) {
      this.fetchLogDetails(this.logId);
    }
  }
  fetchLogDetails(id: number | null): void {
    this.isLoading = true;
    if (this.logId != null) {
      this._logService.getLogById(this.logId).subscribe(
        (response: ResponsAPI<object>) => {
          this.logDetails = response.result;
          this.isLoading = false;
        },
        (error) => {
          console.error('Error fetching log details:', error);
          this.errorMessage =
            'Unable to fetch log details. Please try again later.';
          this.isLoading = false;
        }
      );
    }
  }
}
