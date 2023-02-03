import React from 'react'
import { useState, usecontext } from 'react'
import {Link, useHistory, useNavigate } from 'react-router-dom';
import { Form, Button } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

const LoginPage = ({onLogin}) => {

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const USER_REGEX = /^[A-z][A-z0-9-_]{3,23}$/;
    const PWD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%]).{8,24}$/;

    const navigate = useNavigate();
    const onSubmit = (event) => {
        event.preventDefault()//prevent default behavior for forms
    
        if(!username)
        {
            alert('Unesi username')
            return
        }

        //onLogin({'user': username,'pass': password})
        goFetch({'username': username,'pass': password})

        setPassword('');
        
    } 

    const goFetch = async (user) =>
    {   
        const requestOpt = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json'},
            body: JSON.stringify({ 'username': user.username, 'sifra': user.pass})
        }
        const userN = user.username
        const response = await fetch('https://localhost:7013/Korisnik/LogIn', requestOpt)
        if(response.ok){

            const obj = await response.json();
            

            onLogin(obj.token, obj.username,{ user : obj.username, role : obj.role, accessToken : obj.token } );

            navigate('/')

        }else{
            alert("wrong pass or username")
        }
        //.then(() => {history.replace('/')})
    }

    return (
        <Form className='login-form' onSubmit={onSubmit}>

            <Form.Group style={{padding:"10px"}}>

                <Form.Label>Username </Form.Label>

                <Form.Control type='text' placeholder='Type here...'
                    value = {username} onChange= { (e) => 
                    setUsername(e.target.value) }/>

            </Form.Group>

            <Form.Group style={{padding:"10px"}}>

                <Form.Label>Password </Form.Label>

                <Form.Control type='password' placeholder='●●●●●●●●●'
                    value = {password} onChange= { (e) => 
                    setPassword(e.target.value) }/>

            </Form.Group>

            <Button variant='dark' style={{width:"50%"}} type='submit' className='button'>Login</Button>
            <Link to="/RegisterPage">Register instead?</Link>

        </Form>
    )

}

export default LoginPage