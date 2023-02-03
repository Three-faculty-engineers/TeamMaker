import React from 'react'
import CreateTask from './CreateTask'
import { useEffect, useState } from 'react'
import { Form, Button, Col, Container, Row } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import RigthArrow from '../img/icons8-right-button-48.png'

function Backlog(props) {
   
    const [opis, setOpis] = useState("")
    const [status, setStatus] = useState(0)
    const [startsprint, setStartSprint] = useState("")
    const [endsprint, setEndSprint] = useState("")
    const [token, setToken] = useState("")
    const [array1, setArray1] = useState([])
    const [array2, setArray2] = useState([])
   
    const onSubmit = (event) => {
        event.preventDefault()
    
        if(opis == "")//jos provera
          return
    
        const request = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${token}`},
          body: JSON.stringify({ 'opis' : opis, 'status' : status, 'startsprint': startsprint, 'endsprint': endsprint, 'taskovi': array2})
        } 
    
        fetch(`https://localhost:7013/Sprint/PostSprint/${props.teamID}`, request).then(response => {
          if(response.ok)
          {
            alert("sprint je uspesno dodat")
            props.setSidebarState(2)
          }
          else
            console.log(request)
        })
        
    
    }
    
    useEffect(() => {
      
      setToken(sessionStorage.getItem("jwt"))

      const request = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
      } 
      fetch(`https://localhost:7013/Task/GetTasksWithStatus/${props.teamID}/${0}`, request).then(response => {
        if(response.ok)
          response.json().then((tasks) => {
            setArray1(tasks)
          })
      })

    }, [])


    const onClick = (task) => {
      
      setArray2([...array2, task])
      setArray1(array1.filter((t) => t.ime !== task.ime))

    }
    const onClickReturn = (task) => {
      
      setArray1([...array1, task])
      setArray2(array2.filter((t) => t.ime !== task.ime))

    } 

    const addTask = (task) => {
      setArray1([...array1, task])//ovde nastaje bug zato sto task jos uvek nema dodeljeni task, vrv morace das se zove nazad iz baze
    }
    return (
        <Container fluid>
            <Row>
                <Col>
                  <Form width='100%'className='taskForm form2' onSubmit={onSubmit}>

                    <Form.Group className='backlog'>

                      <h3>Sprint task list:</h3>
                      <div> {array2.map( (task) => ( <div className='TaskOnList Right' key={task.id} onClick={() => {onClickReturn(task)}}> {task.opis} - {task.ime} <img className ="arrowLeft" src={RigthArrow} alt=">" /> </div> ) )} </div>  

                    </Form.Group>

                    <Form.Group className='form-cont'>
                      <Form.Label>Description</Form.Label>

                      <Form.Control type='text' placeholder='opis'
                          value = {opis} onChange= { (e) => 
                            setOpis(e.target.value) }/>
                    </Form.Group>


                    <Form.Group className='form-cont'>
                      <Form.Label>Start date</Form.Label>

                      <Form.Control type='date' 
                          onChange= { (e) => 
                          setStartSprint(e.target.value) }/>
                    </Form.Group>

                    <Form.Group className='form-cont'>
                      <Form.Label>End date</Form.Label>

                      <Form.Control type='date' 
                          onChange= { (e) => 
                          setEndSprint(e.target.value) }/>
                    </Form.Group>

                      <Button variant='dark' className='button' type='submit'>Create</Button>

                  </Form>
                </Col>

                <Col>                            
                    <CreateTask onKlik={onClick} addTask ={addTask} array1 = {array1} teamID={props.teamID}/>
                </Col>
            </Row>
        </Container>
        
    )
}

export default Backlog