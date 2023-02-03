import React from 'react'
import { useState } from 'react'
import {Link, useHistory, useNavigate } from 'react-router-dom';
import { Form, Button } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

const RegisterPage = (props) => {

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [fakultet, setFakultet] = useState('');
    const [indeks, setIndeks] = useState('');

    const [name, setName] = useState('');
    const [surname, setSurname] = useState('');
    const [mail, setMail] = useState('');

    const navigate = useNavigate();


    const onSubmit = (event) => {
        event.preventDefault()
    
        if(!username || !password ||
        !surname || !mail ||
        !indeks || !name || !fakultet  )
        {
            alert('Izostavili ste jedno polje ili vise polja')
            return
        }

        goFetch({'username': username,'password': password, 'fakultet': fakultet,
    'indeks': indeks, 'name': name, 'surname': surname, 'mail': mail});

        setUsername('');
        setPassword('');
        setFakultet('');
        setIndeks('');
        setName('');
        setSurname('');
        setMail('');
    } 

    const goFetch = async (user) =>
    {   
        const requestOpt = {            
        method: "POST",
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 'username': user.username, 'sifra': user.password,
        'fakultet': user.fakultet, 'indeks': user.indeks,
        'ime': user.name, 'prezime': user.surname, 'email': user.mail})
    };

        const response = await fetch("https://localhost:7013/Korisnik/Register", requestOpt)
        if(response.ok){
            alert("Uspešno ste se registrovali!")
            navigate('/')
        }else{
            alert("Doslo je do greske")
        }

       
    }

    return (
      
        <Form className='register-form' onSubmit={onSubmit}>
            <Form.Group style={{padding:"10px"}}>

            <Form.Label>Username </Form.Label>

            <Form.Control type='text' placeholder='Type here...' value = {username} onChange= { (e) => 
                    setUsername(e.target.value) } />

        </Form.Group>

        <Form.Group style={{padding:"10px"}}>

            <Form.Label>Password </Form.Label>

            <Form.Control type='password' placeholder='●●●●●●●●●' value = {password} onChange= { (e) => 
                    setPassword(e.target.value) } />

        </Form.Group>        
        

        <Form.Group style={{padding:"10px"}}>
        <Form.Label>Fakultet </Form.Label>
        
        <Form.Control type='text' placeholder='Type here...' value = {fakultet} onChange= { (e) => 
                    setFakultet(e.target.value) } />
        
        </Form.Group>

        <Form.Group style={{padding:"10px"}}>
        <Form.Label>Indeks </Form.Label>
        
        <Form.Control type='text' placeholder='Type here...' value = {indeks} onChange= { (e) => 
                    setIndeks(e.target.value) } />
        
        </Form.Group>

        <Form.Group style={{padding:"10px"}}>
        <Form.Label>Ime </Form.Label>
        
        <Form.Control type='text' placeholder='Type here...' value = {name} onChange= { (e) => 
                    setName(e.target.value) } />
        
        </Form.Group>

        <Form.Group style={{padding:"10px"}}>
        <Form.Label>Prezime </Form.Label>
        
        <Form.Control type='text' placeholder='Type here...' value = {surname} onChange= { (e) => 
                    setSurname(e.target.value) } />
        
        </Form.Group>

        <Form.Group style={{padding:"10px"}}>
        <Form.Label>E-mail </Form.Label>
        
        <Form.Control type='email' placeholder='Ex. someone@domain.com' value = {mail} onChange= { (e) => 
                    setMail(e.target.value) } />
        
        </Form.Group>


        <Button variant='dark' style={{width:"50%"}} type='submit' className='button'>Register</Button>
        <Link to="/">Already have an account?</Link>
    </Form>        
    )

}

export default RegisterPage