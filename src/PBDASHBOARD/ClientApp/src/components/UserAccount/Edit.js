import React, {Component} from "react";
import axios from "axios";


export default class Editing extends Component {
    constructor(props) {
        super(props);

        this.state = {
            email: "",
            ToGroup: "",
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleChange = this.handleChange.bind(this);
    }
    handleChange=(event)=> {
        this.setState({
            [event.target.name]: event.target.value
        });
    }

    async handleSubmit(event) {

        event.preventDefault();
        const {email, ToGroup} = this.state;
        await axios
            .post(window.location.origin +"/AuthMangement/Login", {
                    email: email,
                    ToGroup: ToGroup,
                }, {withCredentials: true}

            ).then(response => {
                console.log("The login process is successed ");
                if (response.data.success === true) {
                    this.props.history.push("/");
                    event.preventDefault();}}).catch(error => {
                console.log("login error", error);
            });


    }

    render() {
        let user = localStorage.getItem('user');
        if (!user) {
            this.props.history.push("/Login")
        }
        return (
            <div style={{padding: '15rem', width: 800, marginBottom: '2rem', marginLeft: '12rem', height: '250px',}}>
                <form onSubmit={this.handleSubmit}>

                    <h3>Edit </h3>

                    <div className="form-group">
                        <label> Contributor's Email</label>
                        <input type="email" className="form-control" name="email" placeholder="Email"
                               value={this.state.email} onChange={this.handleChange} required/>
                    </div>
                    <div className="form-group"><label>new group</label>
                        <input type="tex" className="form-control" name="ToGroup" placeholder="new group"
                               value={this.state.ToGroup} onChange={this.handleChange} required/></div>
                    <button type="submit" className="btn btn-dark btn-lg btn-block">Submit</button>
                </form>
            </div>
        );
    }
}