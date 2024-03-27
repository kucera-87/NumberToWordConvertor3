import { Component } from '@angular/core';
import { AppService, ConvertorResponse } from './app.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = 'Number to Word Converter';
  inputValue: string = '';
  responseData: ConvertorResponse = { output: '', message: '' };
  
  constructor(private appService: AppService) { }

  convertInput(input: string) {

    this.responseData.message = "";
    this.responseData.output = "";

    if (!input || input.trim() === '') {
      return;
    }

    const pattern = /^(?:0|[1-9]\d{0,8})?(?:,\d{0,2})?$/;

    if (!pattern.test(input)) {
      this.responseData.message = "The number is not in valid format!";
      return;
    }

    this.appService.convertInput(input).subscribe(
      (response) => {
        this.responseData = response;
      },
      (error) => {
        console.error(error);
      }
    );
  }
}


