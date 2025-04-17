interface IResultSet<T> {
    amount: number;
    resultDto: T[] | [] | T;
}

interface IHttpResponse<T> {
    isSuccess: boolean;
    value: IResultSet<T>;
    error: string;
}
