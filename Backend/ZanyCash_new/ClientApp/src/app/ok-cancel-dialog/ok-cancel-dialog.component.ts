import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface OkCancelDialogData {
  caption: string;
  text: string;
}

@Component({
  selector: 'app-ok-cancel-dialog',
  templateUrl: './ok-cancel-dialog.component.html',
  styleUrls: ['./ok-cancel-dialog.component.scss']
})
export class OkCancelDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<OkCancelDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: OkCancelDialogData) {}

  ngOnInit(): void {
  }

}
