import {object} from "prop-types";

class HttpServices {
    private api_host: string | undefined;
    
    constructor() {
        this.api_host = process.env.API_HOST;
    }

    async callAPI(route: string, data: object | null | undefined, method: "POST" | "GET" | "DELETE", token: string | null = null) {
           let headers = new Headers();
           headers.append("Access-Control-Allow-Origin", process.env.LocalHost ?? "");
           headers.append("Content-Type", "text/json");

           if (token){
               headers.append("Authorization", `Bearer ${token}`);
           }

           let options: RequestInit;
           if (method !== "GET" && data){
               options = {
                   body: JSON.stringify(data),
                   headers: headers,
                   method : method,
                   mode   : "cors"
               };
           } else {
               options = {
                   headers: headers,
                   method : method,
                   mode   : "cors"
               };
           }

           return await fetch(`${this.api_host}${route}`, options);
    }
}

export default HttpServices;