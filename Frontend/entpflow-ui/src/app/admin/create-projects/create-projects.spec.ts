import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateProjects } from './create-projects';

describe('CreateProjects', () => {
  let component: CreateProjects;
  let fixture: ComponentFixture<CreateProjects>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateProjects]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateProjects);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
