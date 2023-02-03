import React from 'react'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom';

import { Form, Button } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

function CreateTeamPage(props) {

    const navigate = useNavigate();
    const [name, setName] = useState('')
    const [opis, setOpis] = useState('')
    
    const onSubmit = (event) => {
        event.preventDefault()
    
        if(name === '')
        {
            alert('Unesi ime tima')
            return
        }

        goFetch()
        setName('')
        
    } 

    const goFetch = async () =>
    {   
        const requestOpt = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`},
            body: JSON.stringify({ 'Ime': name, 'Opis': opis })
        }
        
        const response = await fetch(`https://localhost:7013/Team/CreateTeam`, requestOpt).then(r => {
            if(r.ok)
                alert('Tim je uspesno kreiran')
                else
                alert('Greska!')
            
        })
        //const data = await response.json()
        navigate('/')
    }


    return (
    <Form className='createTeam-form' onSubmit={onSubmit} >

        <Form.Group className='form-cont'>
            <Form.Label>Team name: </Form.Label>
            <Form.Control type='text' placeholder='Naziv Tima'
                value = {name} onChange = { (e) =>setName(e.target.value)} />    
        </Form.Group>
        <Form.Group className='form-cont'>
            <Form.Label>Description: </Form.Label>
            <Form.Control type='text' placeholder='Opis'
                value = {opis} onChange = { (e) =>setOpis(e.target.value)} />    
        </Form.Group>    

        <Button variant='dark' type='submit' style={{width:"50%"}} className='button'>Create</Button>
    </Form>
  )
}

export default CreateTeamPage
