import React, {Component} from "react";
export default class Logout extends Component{
    constructor(props) {
        super(props);
        this.state={
            isLogout:false};
        this.handleLogout = this.handleLogout.bind(this);};
    render() {
        return(
            <div>{this.handleLogout()}</div>)}
    handleLogout=()=>{
        this.props.history.push("/");
        this.props.handleLogin(false);
        localStorage.clear();}

}